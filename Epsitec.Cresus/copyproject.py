import os
import os.path
import shutil
import time
import popen2

class CopyProject:
    def __init__(self, src_root, dst_root):
        self.src_root = src_root
        self.dst_root = dst_root

        self.copy_ext = list()

        self.copy_ext.append('.cs')
        self.copy_ext.append('.csproj')
        self.copy_ext.append('.bat')
        self.copy_ext.append('.ico')
        self.copy_ext.append('.txt')
        self.copy_ext.append('.png')
        self.copy_ext.append('.py')
        self.copy_ext.append('.snk')
        self.copy_ext.append('.sln')
        self.copy_ext.append('.config')
        self.copy_ext.append('.info')

        self.file_count = 0
        self.proj_count = 0
        self.sol_count = 0
        self.bad_count = 0
    
    def strip_scc_info(self, name):
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
        for ext in ['.dll','.pdb','.scc','.user','.exe','.vspscc','.vssscc','.projdata','.suo']:
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
                        self.strip_scc_info(dst_name)

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

def analyse(arg, dirname, fnames):
    for name in fnames:
        arg.copy_file(dirname, name)




src = "C:\\Documents and Settings\\Arnaud\\My Documents\\Visual Studio Projects\\Epsitec.Cresus"
dst = "C:\\Epsitec"
zip = "Epsitec.Cresus.zip"



copy_project = CopyProject (src, dst)

os.path.walk(src,analyse,copy_project)
copy_project.print_statistics()




zip = dst + '\\' + zip

try:
    os.remove(zip)
except OSError, e:
    e = ''

wzzip = 'c:\\Progra~1\\winzip\\wzzip'
opt   = '-a -p -r'
qzip  = '"' + zip + '"'
what  = '"' + dst + '\\*.*"'

cmd   = wzzip+' '+opt+' '+qzip+' '+what

r,w,e = popen2.popen3 (cmd)
result = r.read()
errors = e.read()

if os.path.isfile(zip):
    print 'Created file '+zip
else:
    print 'error: ZIP file not created'
    time.sleep (2)
