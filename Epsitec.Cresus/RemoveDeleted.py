# -*- coding: cp1252 -*-
#########################################################################
# This Python script is used to remove files marked as ~(deleted)    	#
# from the 'copyproject-py' script.              			#
#									#
# (C) Copyright 2004, Denis DUMOULIN                               	#
#########################################################################

import os
import os.path
import shutil
import time
import popen2
import time
import string
import difflib
import socket

class RemoveDelProject:
    def __init__(self, src_root):
        self.src_root = src_root

    def remove_deleted(self, name):
        if name.endswith('~(deleted)'):
            print "remove " + name.replace(self.src_root,'') ;
            os.remove(name);
            try:
                name = name.replace('~(deleted)', '');
                os.remove(name);
                print name.replace(self.src_root,'') + " deleted";
            except OSError, e:
                e = ''
                      
    def remove_file(self, path, name):
        if path.find(self.src_root) > -1:
            src_name = path + "\\" + name

            if os.path.isdir(src_name) == 0:
                if name.endswith('~(deleted)'):
                    self.remove_deleted(src_name)


def remove_analyse(arg, dirname, fnames):
#   print   "Searching in " + dirname;
    for name in fnames:
        arg.remove_file(dirname, name)


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

    patch_project = RemoveDelProject (src)

    print "Searching tree..."
    os.path.walk(src,remove_analyse,patch_project)

    print "\nDone."
    time.sleep (3)


# run the script...

do_it()
