import os
import subprocess
import sys
import shutil
def print_error(msg):
    print(msg, file=sys.stderr)
    exit(-1)
def build() -> None:
    buildSrcPath, newBuildSourcePath = '/BuildSource', '/BuildSourceCopy'
    buildTargetPath = '/BuildOutput'
    if os.path.exists(buildSrcPath) == False:
        print_error(f"{buildSrcPath} doesn't exist.")
        
    if os.path.exists(buildTargetPath) == False:
        print_error(f"{buildTargetPath} doesn't exist.")

    shutil.copytree(buildSrcPath, newBuildSourcePath)
    subprocess.run(['chmod', '-R', '+x', newBuildSourcePath], check=True)
    
    p = subprocess.run([os.path.join(newBuildSourcePath, 'build.sh'), buildTargetPath], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)

    if p.returncode != 0:
        print_error('build.sh returned non-zero code.')
    
    if len(os.listdir(buildTargetPath)) == 0:
        print_error('Build output directory is empty.')

def run_checker() -> None:
    checkerPath, newCheckerPath = '/Checker', '/CheckerCopy'
    submissionPath, newSubmissionPath='/Submission','/SubmissionCopy'
    resultDirectoryPath = '/Result'
    if os.path.exists(checkerPath) == False:
        print_error(f"{checkerPath} doesn't exist.")

    if os.path.exists(submissionPath) == False:
        print_error(f"{submissionPath} doesn't exist.")

    if os.path.exists(resultDirectoryPath) == False:
        print_error(f"{resultDirectoryPath} doesn't exist.")

    shutil.copytree(checkerPath, newCheckerPath)
    shutil.copytree(submissionPath, newSubmissionPath)
    subprocess.run(['chmod', '-R', '+x', newCheckerPath], check=True)
    subprocess.run(['chmod', '-R', '+x', newSubmissionPath], check=True)


    p = subprocess.run([os.path.join(newCheckerPath, 'init.sh')], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if p.returncode != 0:
        print_error(f'init.sh returned {p.returncode}.')

    p = subprocess.run([os.path.join(newCheckerPath, 'run.sh'), newSubmissionPath], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True)
    if p.returncode != 0:
        print_error(f'run.sh returned {p.returncode}.')
    with open(os.join(resultDirectoryPath, 'result.txt'), 'w') as fs:
        fs.write(p.stdout)
        fs.flush()

if len(sys.argv) < 2:
    print_error('Insufficient number of arguments.')

if len(sys.argv) > 2:
    print_error('Unexpected number of arguments.')

if(sys.argv[1] == 'build'):
    build()
    exit(0)


if(sys.argv[1] == 'check'):
    run_checker()
    exit(0)

print_error('Unexpected command.')
