import os
import subprocess
import socket
import sys
import time
import signal
import shutil
import json
from pathlib import Path
import platform

def print_error(err: str) -> None:
    print(err, file=sys.stderr)
    exit(0)

def print_info(info: str) -> None:
    print(info)

def check_ports() -> None:
    def check_port(port: int) -> bool:
        try:
            with socket.socket(socket.AddressFamily.AF_INET, socket.SOCK_STREAM) as sok:
                sok.bind((socket.gethostname(),port))
                sok.close()
        except Exception:
            return False
        return True

    ports = [(1234, 'Backend API'), (1235, 'Docker broker push socket'), (1236, 'Docker broker pull port'), (1237, 'Docker broker API'),]
    for p in ports:
        if not check_port(p[0]):
            print_error(f"Port {p[0]} used for '{p[1]}' is currently used by another process")
def check_dependecies() -> None:
    docker = subprocess.run(['docker', '-v'], stdout = subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if docker.returncode != 0:
        print_error("Docker is not installed")
    
    docker_compose = subprocess.run(['docker-compose', '-v'], stdout = subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if docker.returncode != 0:
        docker_compose("Docker-Compose is not installed")

    dotnet = subprocess.run(['dotnet', '--list-sdks'], stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
    if dotnet.returncode != 0:
        print_error('.NET is not installed')
    if any(i.startswith('5') for i in dotnet.stdout.splitlines()) == False:
        print_error('.NET sdk 5 is not installed')

print_info('Checking dependencies.')
check_dependecies()

print_info('Checking ports.')
check_ports()

print_info('Checking directories')
save_directory = Path("~/Desktop").expanduser().joinpath("GraduationProject")
data_directory = save_directory.joinpath("Data")

if data_directory.exists() == False:
    data_directory.mkdir(parents=True)

print_info('Building docker client')
cd = Path(__file__).parent
client_project_path = cd.joinpath('DockerClient')
client_publish_path = save_directory.joinpath('DockerClient')

if client_publish_path.exists():
    shutil.rmtree(str(client_publish_path))

client_build_process = subprocess.run(['dotnet', 'publish', str(client_project_path), '-c', 'Release', '-o', str(client_publish_path)], stdout=subprocess.PIPE, stderr=subprocess.STDOUT, text=True)
if client_build_process.returncode != 0:
    print_error("Couldn't build Docker Client.")

client_app_settings = client_publish_path.joinpath('appsettings.json')

app_settings = json.loads(client_app_settings.read_text())
app_settings['App']['DataSaveDirectory'] = str(data_directory)
app_settings['App']['CentralDataDirectory'] = str(data_directory)
client_app_settings.write_text(json.dumps(app_settings))

print_info('Building sandbox')
docker_client = subprocess.Popen(['dotnet', str(client_publish_path.joinpath('DockerClient.dll')), '-v'], cwd =str(client_publish_path))



print_info('Starting Backend API, Docker Broker and DB server')
original_docker_compose = cd.joinpath('original-docker-compose.yml')
new_docker_compose = original_docker_compose.with_stem('docker-compose')

user_id, group_id = '1000', '1000'
if platform.system().lower() == 'linux':
    user_id = subprocess.run(['id', '-u'], capture_output=True, text=True).stdout.strip()
    group_id = subprocess.run(['id', '-g'], capture_output=True, text=True).stdout.strip()


new_docker_compose.write_text(original_docker_compose.read_text()
.replace('#BackendHostDataDirectory#', str(data_directory))
.replace('#HostUserId#', user_id)
.replace('#HostGroupId#', group_id)
)

subprocess.run(['docker-compose', '-f', str(new_docker_compose), 'build'])

compose = subprocess.Popen(['docker-compose', '-f', str(new_docker_compose), 'up'])

time.sleep(10)

print_info('Starting docker client')
docker_client = subprocess.Popen(['dotnet', str(client_publish_path.joinpath('DockerClient.dll'))], cwd =str(client_publish_path))



terminate = False

def handler_stop_signals(signum, frame):
    global terminate
    terminate = True

signal.signal(signal.SIGINT, handler_stop_signals)
signal.signal(signal.SIGTERM, handler_stop_signals)

while not terminate:
    time.sleep(1)

print_info('Stopping Backend API, Docker Broker and DB server')
compose.terminate()

print_info('Stopping Docker Client')
docker_client.terminate()

print_info('C Ya')