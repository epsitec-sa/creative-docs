#########################################################################
# This Python script is used to copy the contents of the Epsitec.Cresus	#
# solution to a clean folder and generate a ZIP file.					#
#																		#
# (C) Copyright 2003, Pierre ARNAUD, EPSITEC SA, Ch. du Fontenay 6,		#
#					  CH-1400 Yverdon-les-Bains, Switzerland			#
#########################################################################

import os
import os.path
import shutil
import time
import popen2
import time
import string
import difflib

class CopyProject:
    def __init__(self, src_root, dst_root):
        self.src_root = src_root
        self.dst_root = dst_root

        self.copy_ext = list()

        self.copy_ext.append('.cs')
        self.copy_ext.append('.csproj')
        self.copy_ext.append('.csproj.user')
        self.copy_ext.append('.bat')
        self.copy_ext.append('.ico')
        self.copy_ext.append('.txt')
        self.copy_ext.append('.png')
        self.copy_ext.append('.icon')
        self.copy_ext.append('.tif')
        self.copy_ext.append('.chm')
        self.copy_ext.append('.py')
        self.copy_ext.append('.resource')
        self.copy_ext.append('.snk')
        self.copy_ext.append('.sln')
        self.copy_ext.append('.config')
        self.copy_ext.append('.info')
        self.copy_ext.append('.log')
        self.copy_ext.append('.vdproj')

        self.file_count = 0
        self.proj_count = 0
        self.sol_count = 0
        self.bad_count = 0

    def current_time(self):
        return ''.join ((string.zfill(str (time.localtime(time.time()).tm_mday), 2), '/',
                         string.zfill(str (time.localtime(time.time()).tm_mon), 2), '/',
                         str (time.localtime(time.time()).tm_year), ' ',
                         string.zfill(str (time.localtime(time.time()).tm_hour), 2), ':',
                         string.zfill(str (time.localtime(time.time()).tm_min), 2)))
    
                         

    def strip_scc_info_csproj(self, name):
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

        self.proj_count += 1

    def strip_scc_info_vdproj(self, name):
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
        f = open(name, 'r')
        lines = f.readlines()
        lines_to_remove = list()
        f.close()

        i = 0
        while i < len(lines):
            if lines[i].find('C:\\Documents and Settings\\Arnaud\\My Documents\\Visual Studio Projects\\') > -1:
                lines[i] = lines[i].replace ('C:\\Documents and Settings\\Arnaud\\My Documents\\Visual Studio Projects\\', 'D:\\Cresus\\')
            i += 1
        
        f = open(name, 'w')
        f.writelines(lines)
        f.close()

        self.proj_count += 1

    def strip_solution(self, name):
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

        self.sol_count += 1

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
            dst_path = path.replace (self.src_root, self.dst_root)
            dst_name = dst_path + "\\" + name

            if name == 'copy.info':
                f = open(src_name, 'r')
                files = f.readlines()
                f.close()
                for file in files:
                    if (file.strip().startswith('#') == 0) & (len(file.strip())>0):
                        self.copy_file(path, file.strip(), 1)

            if os.path.isdir(src_name):
                if os.path.isdir(dst_name) == 0:
                    os.makedirs(dst_name)
            else:
                if force_copy | self.check_copy(src_name):
                    f = open(src_name, 'rb')
                    data = f.read()
                    f.close()
                    f = open(dst_name, 'wb')
                    f.write(data)
                    f.truncate()
                    f.close()

                    if name.endswith('.csproj'):
                        self.strip_scc_info_csproj(dst_name)
                    if name.endswith('.vdproj'):
                        self.strip_scc_info_vdproj(dst_name)
                    if name.endswith('.csproj.user'):
                        self.patch_path_info(dst_name)
                    if name.endswith('.sln'):
                        self.strip_solution(dst_name)

                    shutil.copystat(src_name, dst_name)
                    os.chmod(dst_name, 0777)

                    self.file_count += 1
                    
                else:
                    if self.check_no_copy(dst_name) == 0:
                        print "What to do with " + dst_name
                        self.bad_count += 1

    def print_statistics(self):
        if self.sol_count > 1:
            plural_solutions = 's'
        else:
            plural_solutions = ''
        
        print "Copied %(files)d files with %(proj)d projects in %(sol)d solution%(plurs)s" % {
            'files': self.file_count,
            'proj':  self.proj_count,
            'sol':   self.sol_count,
            'plurs': plural_solutions }
        
        if self.bad_count == 1:
            print "Failed to process 1 file"
            time.sleep(2)
        
        if self.bad_count > 1:
            print "Failed to process %(n)d files" % { 'n': self.bad_count }
            time.sleep(2)



class DiffProject:
    def __init__(self, new_root, ref_root, diff_file):
        self.new_root = new_root
        self.ref_root = ref_root
        self.diff = diff_file

    def diff_file(self, path, name):
        new_path = path
        ref_path = path.replace (self.new_root, self.ref_root)
        new_file = new_path + "\\" + name
        ref_file = ref_path + "\\" + name
        if os.path.isfile(ref_file) & os.path.isfile(new_file):
            f = open(ref_file, 'r')
            ref_data = f.read()
            f.close()
            f = open(new_file, 'r')
            new_data = f.read()
            f.close()
            if ref_data <> new_data:
                print "Update " + new_file.replace (self.new_root, ".")
                self.diff.write ("  " + new_file.replace (self.new_root, ".") + '\n')
            else:
                os.remove(new_file)
        else:
            if os.path.isfile(new_file):
                print "New " + new_file.replace (self.new_root, ".")
                self.diff.write ("  " + new_file.replace (self.new_root, ".") + '\t(new)\n')
            


def copy_analyse(arg, dirname, fnames):
    for name in fnames:
        arg.copy_file(dirname, name)


def diff_analyse(arg, dirname, fnames):
    for name in fnames:
        arg.diff_file(dirname, name)


def do_it(src,dst,zip):

    copy_project = CopyProject (src, dst + "\\clean")

    print "Copying clean source tree..."
    os.path.walk(src,copy_analyse,copy_project)
    copy_project.print_statistics()
    print "Done."


src = "C:\\Documents and Settings\\Arnaud\\My Documents\\Visual Studio Projects\\Epsitec.Cresus"
dst = "C:\\Epsitec"

# run the script...

do_it(src,dst,zip)
