# -*- coding: cp1252 -*-
#########################################################################
# This Python script is used to copy the contents of the Epsitec.Cresus	#
# solution to a clean folder and generate a ZIP file.			#
#									#
# (C) Copyright 2003, Denis DUMOULIN, thanks to :                     	#
# (C) Copyright 2003, Pierre ARNAUD, EPSITEC SA, Ch. du Fontenay 6,	#
#	              CH-1400 Yverdon-les-Bains, Switzerland		#
#########################################################################

# par rapport au script original, celui-ci ne crée pas de dossier vide
# dans le ZIP des différences.
# par contre il n'enlève pas, dans la référence, les fichiers supprimés


import os
import os.path
import shutil
import time
import popen2
import time
import string
import difflib
import socket

class CopyProject:
    def __init__(self, src_root, ref_root, delta_root, diff_file, history):
        self.src_root = src_root
        self.ref_root = ref_root
        self.delta_root = delta_root
        self.diff = diff_file
        self.history = history

        self.copy_ext = list()

        self.copy_ext.append('.cs')
        self.copy_ext.append('.csproj')
        self.copy_ext.append('.csproj.user')
        self.copy_ext.append('.bat')
        self.copy_ext.append('.ico')
        self.copy_ext.append('.txt')
        self.copy_ext.append('.png')
        self.copy_ext.append('.jpg')
        self.copy_ext.append('.tif')
        self.copy_ext.append('.chm')
        self.copy_ext.append('.py')
        self.copy_ext.append('.resource')
        self.copy_ext.append('.snk')
        self.copy_ext.append('.sln')
        self.copy_ext.append('.config')
        self.copy_ext.append('.info')
        self.copy_ext.append('.icon')
        self.copy_ext.append('.log')
        self.copy_ext.append('.vdproj')

        self.file_count = 0
        self.mod_count = 0
        self.proj_count = 0
        self.sol_count = 0
        self.bad_count = 0
        self.del_count = 0

    def current_time(self):
        return ''.join ((string.zfill(str (time.localtime(time.time()).tm_mday), 2), '/',
                         string.zfill(str (time.localtime(time.time()).tm_mon), 2), '/',
                         str (time.localtime(time.time()).tm_year), ' ',
                         string.zfill(str (time.localtime(time.time()).tm_hour), 2), ':',
                         string.zfill(str (time.localtime(time.time()).tm_min), 2)))
    

    def log_history(self):
        try:
            f = open (self.src_root + self.history, 'rt')
            lines = f.readlines()
            revnum = 1
            for line in lines:
                if line[0:2].isdigit():
                    revnum += 1
                    if line[16:21] == ' Rev ':
                        revnum = int (line[21:]) + 1
            f.close ()
        except IOError, e:
            lines = list()
            revnum = 1

        new_line = self.current_time() + ' Rev ' + str(revnum) + '\n'
        lines.append (new_line)
        f = open (self.src_root + self.history, 'wt')
        f.writelines (lines)
        f.close()
        return revnum
    
    def strip_scc_info_csproj(self, name):
        print "Stripping scc info from " + name
        f = open(name, 'r')
        lines = f.readlines()
        lines_to_remove = list()
        f.close()
        for line in lines:
            if line.strip().startswith("Scc"):
                lines_to_remove.append (line)

        for line in lines_to_remove:
            lines.remove(line)

        i = 0
        while i < len(lines):
            if lines[i].find('..\\..\\..\\..\\..\\..') > -1:
                lines[i] = lines[i].replace ('..\\..\\..\\..\\..\\..', 'C:')
            i += 1
        
        f = open(name, 'w')
        f.writelines(lines)
        f.close()


    def strip_scc_info_vdproj(self, name):
        print "Stripping scc info from " + name
        f = open(name, 'r')
        lines = f.readlines()
        lines_to_remove = list()
        f.close()
        for line in lines:
            if line.strip().startswith('"Scc'):
                lines_to_remove.append (line)

        for line in lines_to_remove:
            lines.remove(line)

        f = open(name, 'w')
        f.writelines(lines)
        f.close()

        self.proj_count += 1

    def patch_path_info(self, name):
        print "Patching path info in " + name
        f = open(name, 'r')
        lines = f.readlines()
        lines_to_remove = list()
        f.close()

        i = 0
        n = 0
        while i < len(lines):
            if lines[i].find(self.src_root) > -1:
                lines[i] = lines[i].replace (self.src_root, 'D:\\Cresus\\Epsitec.Cresus')
                n += 1
            i += 1

        if n > 0:
            print 'replaced directory in ' + name
        
        f = open(name, 'w')
        f.writelines(lines)
        f.close()

        self.proj_count += 1

    def strip_solution(self, name):
        print "Stripping scc info from " + name
        f = open(name, 'r')
        lines = f.readlines()
        f.close()
        f = open(name, 'w')
        write = 1
        for line in lines:
            if write:
                if line.strip().startswith("GlobalSection(SourceCodeControl)"):
                    write=0
                else:
                    f.write(line)
            else:
                if line.strip().startswith("EndGlobalSection"):
                    write=1
        f.close()

    def check_copy(self, name):
        for ext in self.copy_ext:
            if name.endswith(ext):
                return 1
        return 0

    def check_no_copy(self, name):
        for ext in ['.dll','.pdb','.scc','.user','.exe','.vspscc','.vssscc','.projdata','.suo','.projdata1','.mgb','.Exe','.Ini','.msi']:
            if name.endswith(ext):
                return 1

        return 0


    def copy_file(self, path, name, force_copy = 0):
        if path.find(self.src_root) > -1:
            src_path = path
            src_name = src_path + "\\" + name
            ref_path = path.replace (self.src_root, self.ref_root)
            delta_path = path.replace (self.src_root, self.delta_root)
            ref_name = ref_path + "\\" + name
            delta_name = delta_path + "\\" + name

            if os.path.isdir(ref_path) == 0:
                os.makedirs(ref_path)

            if name == 'copy.info':
                f = open(src_name, 'r')
                files = f.readlines()
                f.close()
                for file in files:
                    if (file.strip().startswith('#') == 0) & (len(file.strip())>0):
                        self.copy_file(path, file.strip(), 1)

            if os.path.isdir(src_name) == 0:
                if force_copy | self.check_copy(src_name):
                    f = open(src_name, 'rb')
                    data = f.read()
                    f.close()

                    to_update = 1
                    s1 = "New "
                    s2 = "\t(new)"
                    if os.path.isfile(ref_name):
                        s1 = "Update "
                        s2 = ""
                        f = open(ref_name, 'rb')
                        data2 = f.read()
                        f.close()
                        if data == data2:
                            to_update = 0

                    if to_update > 0:
                        print s1 + src_name.replace (self.src_root, ".")
                        if self.diff:
                            self.diff.write ("  " + src_name.replace (self.src_root, ".") + s2 + '\n')
                        if os.path.isdir(delta_path) == 0:
                            os.makedirs(delta_path)

                        f = open(delta_name, 'wb')
                        f.write(data)
                        f.truncate()
                        f.close()
                        shutil.copystat(src_name, delta_name)
                        os.chmod(delta_name, 0777)

                        if name.endswith('.csproj'):
                            self.strip_scc_info_csproj(delta_name)
                        if name.endswith('.vdproj'):
                            self.strip_scc_info_vdproj(delta_name)
                        if name.endswith('.csproj.user'):
                            self.patch_path_info(delta_name)
                        if name.endswith('.sln'):
                            self.strip_solution(delta_name)

                        os.chmod(delta_name, 0777)

                        f = open(ref_name, 'wb')
                        f.write(data)
                        f.truncate()
                        f.close()

                        shutil.copystat(src_name, ref_name)
                        os.chmod(ref_name, 0777)
                        self.mod_count += 1

                    self.file_count += 1
                    if name.endswith('.sln'):
                        self.sol_count += 1
                    if name.endswith('.csproj'):
                        self.proj_count += 1
                   
                else:
                    if self.check_no_copy(ref_name) == 0:
                        print "What to do with " + ref_name
                        self.bad_count += 1
        return 0

    def delete_file(self, path, name):
        if path.find(self.ref_root) > -1:
            src_path = path.replace (self.ref_root, self.src_root)
            src_name = src_path + "\\" + name
            ref_path = path
            delta_path = path.replace (self.ref_root, self.delta_root)
            ref_name = ref_path + "\\" + name
            delta_name = delta_path + "\\" + name + "~(deleted)"

            if os.path.isdir(src_name) == 0:
                if os.path.isfile(src_name) == 0:
                    print "Delete " + ref_name.replace (self.ref_root, ".")
                    if self.diff:
                        self.diff.write ("  " + src_name.replace (self.src_root, ".") + "\t(del)\n")
                    if os.path.isdir(delta_path) == 0:
                        os.makedirs(delta_path)

                    f = open(delta_name, 'wb')
                    f.truncate()
                    f.close()
                    shutil.copystat(ref_name, delta_name)
                    os.chmod(delta_name, 0777)
                    try:
                        os.remove(ref_name) # ne sait pas supprimer les dossiers
                    except OSError, e:
                        e = ''

                    self.del_count += 1

    def print_statistics(self):
        if self.sol_count > 1:
            plural_solutions = 's'
        else:
            plural_solutions = ''
        
        print "Updated %(mods)d files among %(files)d for %(proj)d projects in %(sol)d solution%(plurs)s" % {
            'mods': self.mod_count,
            'files': self.file_count,
            'proj':  self.proj_count,
            'sol':   self.sol_count,
            'plurs': plural_solutions }

        if self.del_count:
            print "Deleted %(delfiles)d files from references" % {
                'delfiles': self.del_count }
                
        
        if self.bad_count == 1:
            print "Failed to process 1 file"
            time.sleep(2)
        
        if self.bad_count > 1:
            print "Failed to process %(n)d files" % { 'n': self.bad_count }
            time.sleep(2)


def copy_analyse(arg, dirname, fnames):
    for name in fnames:
        arg.copy_file(dirname, name)

def delete_analyse(arg, dirname, fnames):
    for name in fnames:
        arg.delete_file(dirname, name)

def read_localisation_file():
    try:
        f = open ("loc-" + socket.gethostname() + ".txt", 'rt')
        lines = f.readlines()
        f.close
        print 'Using configuration for machine ' + socket.gethostname() + '.'
        return lines
    except OSError, e:
        print 'Machine dependent localisation file not found.'
    
    try:
        f = open ("localisation.txt", 'rt')
        lines = f.readlines()
        f.close;
        return lines
    except OSError, e:
        print   'Error, could not read file "localisation.txt"'
        time.sleep (30)
        sys.exit()


def do_it():
    lines = read_localisation_file()

    src = lines[0].replace("\n", "")        # "F:\\Travail\\Cresus\\Epsitec.Cresus"
    dst = lines[1].replace("\n", "")        # "C:\\Epsitec"
    zip = lines[2].replace("\n", "")        # "Epsitec.Cresus-DD"
    history = "\\" + lines[3].replace("\n", "")    # "history-dd.log"
    wzzip = lines[4].replace("\n", "")      # 'c:\\Progra~1\\winzip\\wzzip'

    diff_log = 0
    try:
        shutil.rmtree(dst + "\\ref-temp")
    except OSError, e:
        e = ''
        
    if os.path.isdir(dst + "\\ref"):
        shutil.copytree(dst + "\\ref", dst + "\\ref-temp")
        diff_log = file (src + history, 'a')
        diff_log.seek(0,2)

    copy_project = CopyProject (src, dst + "\\ref-temp", dst + "\\delta", diff_log, history)
    rev = str(copy_project.log_history())

    try:
        shutil.rmtree(dst + "\\delta")
    except OSError, e:
        e = ''
    os.makedirs(dst + "\\delta")

    print "Updating references tree..."
    os.path.walk(src,copy_analyse,copy_project)

    print "Removing deleted references..."
    os.path.walk(dst+"\\ref-temp",delete_analyse,copy_project)
    copy_project.print_statistics()

    if diff_log == 0:
        diff_log = file (src+history, 'a')
        diff_log.seek(0,2)
        diff_log.write ("  full copy\n")

    diff_log.write ("\n")
    diff_log.truncate()
    diff_log.close ()
    diff_log = 0

    try:
        os.remove (dst + "\\delta" + history)
    except OSError, e:
        e = ''
    try:
        os.remove (dst + "\\ref-temp" + history)
    except OSError, e:
        e = ''

    shutil.copy (src + history, dst + "\\delta" + history)
    shutil.copy (src + history, dst + "\\ref-temp" + history)

    zip = dst + '\\' + zip + rev + '.zip'

    try:
        os.remove(zip)
    except OSError, e:
        e = ''

    print "Zipping deltas..."

    opt   = '-a -p -r'
    qzip  = '"' + zip + '"'
    what  = '"' + dst + '\\delta\\*.*"'

    cmd   = wzzip + ' ' + opt + ' ' + qzip + ' ' + what

    if copy_project.mod_count > 100 :   # wzzip se bloque lorsqu'il y a trop de fichiers
        cmd.replace ("wzzip" , "WINZIP32.EXE")  # utilise winzip32 à la place
    
    print 'MEGABUILD.SET %ziprev%=' + rev
    print 'MEGABUILD.SET %zipfile%=' + zip

    print 'cmd = ' + cmd
    r,w,e = popen2.popen3 (cmd)
    result = r.read()
    errors = e.read()

    if os.path.isfile(zip):
        print 'Created file '+zip
        try:
            print "Removing old references..."
            shutil.rmtree(dst + "\\ref")
        except OSError, e:
            e = ''
        try:
            os.rename(dst+"\\ref-temp", dst+"\\ref")
        except OSError, e:
            e = ''
        try:
            print "Removing delta..."
            shutil.rmtree(dst + "\\delta")
        except OSError, e:
            e = ''
    else:
        print 'error: ZIP file not created'
        try:
            print "Removing temp references..."
            shutil.rmtree(dst + "\\ref-temp")
        except OSError, e:
            e = ''
        print 'References not updated'
        print 'Delta directory not deleted'
        time.sleep (2)

    print "Done."


# run the script...

do_it()
