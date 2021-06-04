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
    result.write(grade)
    result.write('\n')
    result.write(f'Arguments: {" ".join(sys.argv)}\n')
    author_answer = Path(args['submissionPath']).joinpath('author_answer.txt')
    if author_answer.exists():
        result.write('Author answer:\n')
        result.write(author_answer.read_text())
        result.write('\n')
    else:
        result.write('No author result\n')
    result.write('answer.txt content:\n')
    answer = Path(args['submissionPath']).joinpath('answer.txt')
    result.write(answer.read_text())
    result.flush()