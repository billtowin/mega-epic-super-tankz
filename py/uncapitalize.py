import sys
import os
import logging

import glob


def main():
	print(glob.glob(sys.argv[1]));
	for filepath in glob.glob(sys.argv[1]):
		filename = os.path.basename(filepath)
		pathprefix = filepath[:len(filepath) - len(filename)]
		newFilename = filename[0].lower() + filename[1:]
		newFilepath = pathprefix + newFilename
		if(filepath != newFilepath):
			print("Renaming:" + filepath + " to " + newFilepath)
			os.rename(filepath,newFilepath)


if __name__ == "__main__":
	main() 