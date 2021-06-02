import os
import subprocess
import sys
import shutil
from pathlib import Path

hostUserId, hostGroupId = os.getenv('HostUserId'),  os.getenv('HostGroupId')

def print_error(msg):
    print(msg, file=sys.stderr)
    exit(-1)
def build() -> None:
    global hostUserId, hostGroupId
    buildSrcPath, newBuildSourcePath = '/BuildSource', '/BuildSourceCopy'
    buildTargetPath = '/BuildOutput'
    if os.path.exists(buildSrcPath) == False:
        print_error(f"{buildSrcPath} doesn't exist.")
        
    if os.path.exists(buildTargetPath) == False:
        print_error(f"{buildTargetPath} doesn't exist.")

    shutil.copytree(buildSrcPath, newBuildSourcePath)
    subprocess.run(['chmod', '-R', '+rwx', newBuildSourcePath], check=True)
    
    p = subprocess.run([os.path.join(newBuildSourcePath, 'build.sh'), '--outputPath',  buildTargetPath], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=newBuildSourcePath)

    subprocess.run(['chmod', '-R', '+rwx', buildTargetPath], check=True)
    
    if p.returncode != 0:
        print_error(f'build.sh returned {p.returncode}.\nStderr:\n{p.stderr}')
    
    if len(os.listdir(buildTargetPath)) == 0:
        print_error('Build output directory is empty.')

    subprocess.run(['chown', '-R', f'+{hostUserId}:+{hostGroupId}', buildTargetPath], check=True)

def run_checker() -> None:
    global hostUserId, hostGroupId
    checkerPath, newCheckerPath = Path('/Checker'), Path('/CheckerCopy')
    submissionPath, newSubmissionPath= Path('/Submission'), Path('/SubmissionCopy')
    resultDirectoryPath = Path('/Result')
    if checkerPath.exists() == False:
        print_error(f"{checkerPath} doesn't exist.")

    if submissionPath.exists() == False:
        print_error(f"{submissionPath} doesn't exist.")

    if resultDirectoryPath.exists() == False:
        print_error(f"{resultDirectoryPath} doesn't exist.")

    shutil.copytree(str(checkerPath), str(newCheckerPath))
    shutil.copytree(str(submissionPath), str(newSubmissionPath))
    subprocess.run(['chmod', '-R', '+rwx', str(newCheckerPath)], check=True)
    subprocess.run(['chmod', '-R', '+rwx', str(newSubmissionPath)], check=True)
    
    
    p = subprocess.run([str(newCheckerPath.joinpath('init.sh'))], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=str(newCheckerPath))
    if p.returncode != 0:
        print_error(f'init.sh returned {p.returncode}.\nStderr:\n{p.stderr}')
        
    p = subprocess.run([str(newCheckerPath.joinpath('run.sh')), '--submissionPath', str(newSubmissionPath), '--resultPath', str(resultDirectoryPath)], stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, cwd=str(newCheckerPath))
    if p.returncode != 0:
        print_error(f'run.sh returned {p.returncode}.\nStderr:\n{p.stderr}')
    
    if any(True for i in resultDirectoryPath.iterdir()) == False:
        print_error('Result directory is empty after run.sh execution.')

    subprocess.run(['chown', '-R', f'+{hostUserId}:+{hostGroupId}', resultDirectoryPath], check=True)


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
