from word import word
from yufa import yufa

mum = word('abc.txt')
mum.get_label()
yf = yufa(mum.lines)
print(mum.lines)
print(mum.value)
yf.S()

