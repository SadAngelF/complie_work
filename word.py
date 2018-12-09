import sys
import string

class word:
    
    def __init__(self,name):
        self.keywards = {}
        self.lines=[]
        self.value=[]

        # 关键字部分
        self.keywards['auto'] = 101
        self.keywards['break'] = 102
        self.keywards['case'] = 103
        self.keywards['char'] = 104
        self.keywards['const'] = 105
        self.keywards['continue'] = 106
        self.keywards['default'] = 107
        self.keywards['do'] = 108
        self.keywards['double'] = 109
        self.keywards['else'] = 110
        self.keywards['enum'] = 111
        self.keywards['extern'] = 112
        self.keywards['float'] = 113
        self.keywards['for'] = 114
        self.keywards['goto'] = 115
        self.keywards['if'] = 116
        self.keywards['int'] = 117
        self.keywards['long'] = 118
        self.keywards['register'] = 119
        self.keywards['return'] = 120
        self.keywards['short'] = 121
        self.keywards['signed'] = 122
        self.keywards['sizeof'] = 123
        self.keywards['static'] = 124
        self.keywards['struct'] = 125
        self.keywards['switch'] = 126
        self.keywards['typedef'] = 127
        self.keywards['union'] = 128
        self.keywards['unsigned'] = 129
        self.keywards['void'] = 130
        self.keywards['volatile'] = 131
        self.keywards['while'] = 132
        self.keywards['#include'] = 133
        self.keywards['main'] = 134

        # 符号
        self.keywards['+'] = 201
        self.keywards['-'] = 202
        self.keywards['*'] = 203
        self.keywards['/'] = 204
        self.keywards['='] = 205
        self.keywards[':'] = 206
        self.keywards['<'] = 207
        self.keywards['>'] = 208
        self.keywards['%'] = 209
        self.keywards['&'] = 210
        self.keywards['!'] = 211
        self.keywards['('] = 212
        self.keywards[')'] = 213
        self.keywards['['] = 214
        self.keywards[']'] = 215
        self.keywards['{'] = 216
        self.keywards['}'] = 217
        self.keywards['#'] = 218
        self.keywards['|'] = 219
        self.keywards[','] = 220
        self.keywards[';'] = 221
        self.keywards['\''] = 222
        self.keywards['\"'] = 223
        self.keywards['!='] = 224
        self.keywards['<>'] = 224
        self.keywards['<='] = 225
        self.keywards['=<'] = 225
        self.keywards['=>'] = 226
        self.keywards['>='] = 226
        self.keywards['=='] = 227
        
        # 变量
        # self.keywards['var'] = 301

        # 常量
        # self.keywards['const'] = 401

        # Error
        # self.keywards['const'] = 501

        self.signlist = {}
        self.file=name

    def pretreatment(self,file_name):
        try:
            fp_read = open(file_name, 'r')
            fp_write = open('file.tmp', 'w')
            sign = 0
            while True:
                read = fp_read.readline()
                if not read:
                    break
                length = len(read)
                i = -1
                while i < length - 1:
                    i += 1
                    if sign == 0:
                        if read[i] == ' ':
                            continue
                    #if read[i] == '\\':
                    #    break              #！！！！！未解决！！！！解决注释问题，本来是这样，但是c语言中的注释是两个字符以上，因此作出修改，在后面识别注释符
                    if read[i] == ' ':
                        if sign == 1:
                            continue
                        else:
                            sign = 1
                            fp_write.write(' ')
                    elif read[i] == '\t':
                        if sign == 1:
                            continue
                        else:
                            sign = 1
                            fp_write.write(' ')
                    elif read[i] == '\n':
                        if sign == 1:
                            continue
                        else:
                            sign = 1
                            fp_write.write(' ')
                    elif read[i] == '"':
                        fp_write.write(read[i])
                        i += 1
                        while i < length and read[i] != '"':
                            fp_write.write(read[i])
                            i += 1
                        if i >= length:
                            break
                        fp_write.write(read[i])
                    elif read[i] == "'":
                        fp_write.write(read[i])
                        i += 1
                        while i < length and read[i] != "'":
                            fp_write.write(read[i])
                            i += 1
                        if i >= length:
                            break
                        fp_write.write(read[i])
                    else:
                        sign = 3
                        fp_write.write(read[i])
        except Exception:
            print(file_name, ': This FileName Not Found!')


    def save(self,string):
        if string in self.keywards.keys():
            self.lines.append(str(self.keywards[string]))
            self.value.append(string)
            if string not in self.signlist.keys():
                self.signlist[string] = self.keywards[string]
        else:
            try:
                float(string)
                self.save_const(string)
            except ValueError:
                self.save_var(string)


    def save_var(self,string):
        if len(string.strip()) < 1:
            pass
        else:
            self.lines.append('301')
            self.value.append(string)
        if string not in self.signlist.keys():
            if len(string.strip()) < 1:
                pass
            else:
                if self.is_signal(string) == 1:
                    self.signlist[string] = 301
                else:
                    self.signlist[string] = 501


    def save_const(self,string):
        self.lines.append('401')
        self.value.append(string)
        if string not in self.signlist.keys():
            self.signlist[string] = 401


    def save_error(self,string):
        if string not in self.signlist.keys():
            self.signlist[string] = 501


    def is_signal(self,s):
        if s[0] == '_' or s[0] in string.ascii_letters:
            for i in s:
                if i in string.ascii_letters or i == '_' or i in string.digits:
                    pass
                else:
                    return 0
            return 1
        else:
            return 0


    def recognition(self,filename):
        try:
            fp_read = open(filename, 'r')
            string = ""
            sign = 0                        #sign=1处理单引号'，sign=2处理双引号"，sign=3处理双符号的，比如!=,<>,<=,=<,>=,=>,==
            while True:
                read = fp_read.read(1)
                if not read:
                    break
                
                if sign == 3:
                    if read == '=' or read == '<' or read == '>':
                        string += read
                        self.save(string)
                        string = ""
                        sign = 0
                        continue
                    else:
                        self.save(string)
                        string = ""
                        sign = 0

                if read == ' ':
                    if len(string.strip()) < 1:
                        sign = 0
                        pass
                    else:
                        if sign == 1 or sign == 2:
                            string += read
                        else:
                            self.save(string)
                            string = ""
                            sign = 0
                elif read == '(':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save('(')
                elif read == ')':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save(')')
                elif read == '[':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save('[')
                elif read == ']':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save(']')
                elif read == '{':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save('{')
                elif read == '}':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save('}')
                elif read == '<':
                    self.save(string)
                    string = "<"
                    sign = 3
                    #self.save('<')
                elif read == '>':
                    self.save(string)
                    string = ">"
                    sign = 3
                    #self.save('>')
                elif read == ',':
                    self.save(string)
                    string = ""
                    self.save(',')
                elif read == "'":
                    string += read
                    if sign == 1:
                        sign = 0
                        self.save_const(string)
                        string = ""
                    else:
                        if sign != 2:
                            sign = 1
                elif read == '"':
                    string += read
                    if sign == 2:
                        sign = 0
                        self.save_const(string)
                        string = ""
                    else:
                        if sign != 1:
                            sign = 2
                elif read == ':':
                    if sign == 1 or sign == 2:
                        string += read
                    else:
                        self.save(string)
                        string = ""
                        self.save(':')
                elif read == ';':
                    self.save(string)
                    string = ""
                    self.save(';')
                elif read == '+':
                    self.save(string)
                    string = ""
                    self.save('+')
                elif read == '=':
                    self.save(string)
                    string = "="
                    sign = 3
                    #self.save('=')
                elif read == '！':
                    self.save(string)
                    string = "!"
                    sign = 3
                    #self.save('！')
                else:
                    string += read

        except Exception as e:
            print(e)

    def get_label(self):
        if self.file == None:
            print("Please Input FileName")
        else:
            self.pretreatment(self.file)
        self.recognition('file.tmp')
        for i in self.signlist.keys():
            print("(", self.signlist[i], ",", i, ")")
        self.dic_cont={}
        for _ in self.value:
            try:
                float(_)
                if (_.count('.')==1):
                    lei='float'
                    self.dic_cont[_]=[lei,float(_)]
                else:
                    lei='int'
                    self.dic_cont[_]=[lei,int(_)]
            except ValueError:
                pass