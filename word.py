import sys
import string

class word:
    
    def __init__(self,name):
        self.keywards = {}

        # 关键字部分
        self.keywards['False'] = 101
        self.keywards['class'] = 102
        self.keywards['finally'] = 103
        self.keywards['is'] = 104
        self.keywards['return'] = 105
        self.keywards['None'] = 106
        self.keywards['continue'] = 107
        self.keywards['for'] = 108
        self.keywards['lambda'] = 109
        self.keywards['try'] = 110
        self.keywards['True'] = 111
        self.keywards['def'] = 112
        self.keywards['from'] = 113
        self.keywards['nonlocal'] = 114
        self.keywards['while'] = 115
        self.keywards['and'] = 116
        self.keywards['del'] = 117
        self.keywards['global'] = 118
        self.keywards['not'] = 119
        self.keywards['with'] = 120
        self.keywards['as'] = 121
        self.keywards['elif'] = 122
        self.keywards['if'] = 123
        self.keywards['or'] = 124
        self.keywards['yield'] = 125
        self.keywards['assert'] = 126
        self.keywards['else'] = 127
        self.keywards['import'] = 128
        self.keywards['pass'] = 129
        self.keywards['break'] = 130
        self.keywards['except'] = 131
        self.keywards['in'] = 132
        self.keywards['raise'] = 133

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
                    if read[i] == '#':
                        break
                    elif read[i] == ' ':
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
                            fp_write.write(' ')
                            sign = 1
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
            if string not in self.signlist.keys():
                self.signlist[string] = self.keywards[string]
        else:
            try:
                float(string)
                self.save_const(string)
            except ValueError:
                self.save_var(string)


    def save_var(self,string):
        if string not in self.signlist.keys():
            if len(string.strip()) < 1:
                pass
            else:
                if self.is_signal(string) == 1:
                    self.signlist[string] = 301
                else:
                    self.signlist[string] = 501


    def save_const(self,string):
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
            sign = 0
            while True:
                read = fp_read.read(1)
                if not read:
                    break

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
                    string = ""
                    self.save('<')
                elif read == '>':
                    self.save(string)
                    string = ""
                    self.save('>')
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
                elif read == '+':
                    self.save(string)
                    string = ""
                    self.save('+')
                elif read == '=':
                    self.save(string)
                    string = ""
                    self.save('=')
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
    
