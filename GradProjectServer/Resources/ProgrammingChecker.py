import random
import sys
from pathlib import Path
def get_args() -> dict:
    args = {}
    i = 1
    while i < len(sys.argv):
        args[sys.argv[i][2:]] = sys.argv[i+1]
        i += 2
    return args

args = get_args()
with open(str(Path(args['resultPath']).joinpath('result.txt')), 'w') as result:
    grade = str(random.random())
    result.write(f'{grade}\n')

    verdicts = ['compilation_error', 'runtime_error', 'time_limit_exceeded', 'wrong_answer', 'partial_accepted', 'accepted']
    verdict = random.choice(verdicts)
    result.write(f'{verdict}\n')

    result.write(f'Arguments: {" ".join(sys.argv)}\n')

    metadata = Path(args['submissionPath']).joinpath('metadata.txt')
    result.write('metadata.txt content:\n')
    result.write(f'{metadata.read_text()}\n')
    
    answer_dir = Path(args['submissionPath']).joinpath('answer')
    result.write('answer directory contents:\n')
    for p in answer_dir.iterdir():
        result.write(f'{str(p)}\n')