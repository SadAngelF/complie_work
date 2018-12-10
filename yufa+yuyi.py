class yufa:
    def __init__(self, signal,signalname,table,command):
        self.signal = signal
        self.signalname = signalname
        self.table = table
        self.command = []
    def getaddress(self):
        return len(self.command)
    def gettopname(self):
        return self.signalname[0]
    def gettop(self):
        return self.signal[0]
    def gettop2(self):
        return self.signal[1]
    def pop(self):
        del self.signal[0]
        del self.signalname[0]
    def lookup(self,name):
        if name in table:
            return 1
        else:
            return 0 
    def S(self):
        self.HEAD()
        if(self.gettop2() != '134'):
            self.VS()
        self.MAIN()
        print("success!")
    def HEAD(self):
        if(self.gettop() == '133'):
            self.pop()
            if(self.gettop() == '207'):
                self.pop()
                if(self.gettop()== '301'):
                    self.pop()
                    if(self.gettop()== '208'):
                        self.pop()
                    else:
                        print("error,缺省> HEAD")
                else:
                    print("error,缺省库名 HEAD")          
            else:
                print("error,缺省< HEAD")    
                exit(0)
            self.HEAD()  
        elif(self.gettop()=='113' or self.gettop()=='109' or self.gettop()=='117'):
            pass
        else:
            print("error, HEAD")
            exit(0)
        print("HEAD!")
    def VS(self):
        ding = self.VS_A()
        name = ding[0]
        attri = ding[1]
        self.VS_B(name,attri)
        if(self.gettop()=='117' and self.gettop2()=='134'):
            pass
        elif(self.gettop()=='117' or self.gettop()=='113' or self.gettop()=='109'):
            self.VS()    
        else:
            print("error,缺少返回值类型 VS")    
            exit(0)
        print("VS!")
    def VS_A(self):
        t = self.TYPE()
        if(self.gettop() == '301'):
            id  = self.gettopname()
            self.pop()
            return (id,t)
        else:
            print("error,变量缺省 VS_A")    
            exit(0)
    def VS_B(self,name,attri):
        if(self.gettop()=='212'):
            self.VS_C()
        else:
            self.VAR_DE_(name,attri)
    def VS_C(self):
        self.pop()        
        self.F_PA()
        if(self.gettop()=='213'):
            self.pop()
            self.VS_D()
        else:
            print("error,期望')' VS_C")
            exit(0)
    def VS_D(self):
        if(self.gettop()=='221'):
            self.pop()
        elif(self.gettop()=='216'):
            self.pop()
            self.FUN_B()
            if(self.gettop()=='217'):
                self.pop()
            else:
                print("error,期望'}' VS_D")
                exit(0)
        else:
            print("error,期望';' VS_D")
            exit(0)
    def FUN_B(self):
        self.VAR_DE()
        self.STATE()
        print("函数主体！")
    def MAIN(self):
        if(self.gettop()=='117'):
            self.pop()
            if(self.gettop()=='134'):
                self.pop()
                if(self.gettop()=='216'):
                    self.pop()
                    self.FUN_B()
                    if(self.gettop()=='217'):
                        self.pop()
                    else:
                        print("error,主函数缺省'}' MAIN")
                        exit(0)    
                else:
                    print("error,主函数缺省'{' MAIN")
                    exit(0)    
            else:
                print("error,期望'mian' MAIN")
                exit(0)    
        else:
            print("error,主函数缺少返回值类型'int' MAIN")
            exit(0) 
        print("mian!")        
    def F_PA(self):
        self.TYPE()
        if(self.gettop()=='301'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                self.F_PA_()
                if(self.gettop()=='213'):
                    self.pop()
                else:
                    print("error,期望')' F_PA")
                    exit(0) 
            else:
                print("error,期望'(' F_PA")
                exit(0)      
        else:
            print("error,函数名错误或缺省 F_PA")
            exit(0) 
    def F_PA_(self):
        if(self.gettop()=='221'):
            self.pop()
            self.F_PA()
        elif(self.gettop()=='213'):
            pass
        else:
            print("error,参数表错误  F_PA_")
            exit(0)
    def VAR_DE(self):
        if(self.gettop()=='117' or self.gettop()=='113' or self.gettop()=='109'):
            attri = self.gettopname()  #属性满足不为空
            self.TYPE()
            if(self.gettop()=='301'):
                name = self.gettopname()    #变量满足不为空
                if(self.lookup(name)):
                    print("此变量已存在")
                    exit(0)
                else:
                    self.table[name] = [attri,'null']    
                self.pop()
                self.VAR_DE_(name,attri)
                self.VAR_DE()
            else:
                print("error,变量定义错误 VAR_DE")
                exit(0)    
        elif(self.gettop()=='301'or self.gettop()=='114' or self.gettop()=='132' or self.gettop()=='116' or self.gettop()=='216' or self.gettop()=='217' or self.gettop()=='120'):
            pass
        else:
            print("error,变量定义错误 VAR_DE")
            exit(0)
        print("变量定义！")
    def VAR_DE_(self,name,attri):
        if(self.gettop()=='221'):
            self.pop()
        elif(self.gettop()=='205'):
            self.pop()
            self.OB(name,attri)
        else:
            print("error,期望';'或变量初始化 VAR_DE_")
            exit(0)
    def STATE(self):
        if(self.gettop()=='217'):
            pass
        else:
            self.B_STATE()
            self.STATE()
    def B_STATE(self):
        if(self.gettop()=='301'):
            if(self.gettop2()=='205'):
                self.A_S()
            elif(self.gettop2()=='212'):
                self.FU_S()
            else:
                print("error,期望变量赋值或函数调用 B_STATE")
                exit(0)
        elif(self.gettop()=='132' or self.gettop()=='108' or self.gettop()=='114'):
            self.CY_S()
        elif(self.gettop()=='120'):
            self.RE_S()
        elif(self.gettop()=='116'):
            self.IF_S()
        elif(self.gettop()=='216'):
            self.MUL_S()
        else:
            print("error,B_STATE")
            exit(0)
    def A_S(self):
        if(self.gettop()=='301'):
            name = self.gettopname()
            self.pop()
            if(self.gettop()=='205'):
                self.pop()
                result = self.OP_S()
                if(result[0] != '_' and result[1] != '_'):
                    if(self.table[result[0]][0] == 'double' or self.table[result[1]][0] == 'double' or self.table[name][0] == 'double'):
                        self.table[name] = ['double',self.table[result[0]][1]+self.table[result[1]][1]]
                    elif(self.table[result[0]][0] == 'float' or self.table[result[1]][0] == 'float' or self.table[name][0] == 'float'):
                        self.table[name] = ['float',self.table[result[0]][1]+self.table[result[1]][1]]    
                    else:
                        self.table[name] = ['int',self.table[result[0]][1]+self.table[result[1]][1]]
                else:
                    if(self.table[name][0] < self.table[result[2][0]]):
                        self.table[name] = [self.table[name][0],self.table[result[2]][0]]
                    else:
                        self.table[name] = [self.table[result[2]][0],self.table[result[2]][0]]         
                self.command[self.getaddress()] = [ '=',result[2],'_',name]    
                if(self.gettop()=='221'):
                    self.pop()
                else:
                    print("error,期望';'")
                    exit(0) 
            else:
                print("error,缺省赋值号 A_S")
                exit(0) 
        else:
            print("error,缺省被赋值变量 A_S")
            exit(0) 
        print("赋值语句！")               
    def OP_S(self):
        name1 = self.OB()
        if(not self.lookup(name1)):
            print("该变量不存在： OP_S")
            print(name1)
            exit(0)
        result = self.OP_S_(name1)
        return result
    def OP_S_(self,name1):
        if(self.gettop()=='201' or self.gettop()=='202' or self.gettop()=='203'or self.gettop()=='204'or self.gettop()=='209'):
            operator = self.OP()
            name2 = self.OB()
            if(not self.lookup(name2)):
                print("此变量不存在：OP_S_")
                print(name2)
                exit(0)
            self.command[self.getaddress()] = [operator,name1,name2,'temp1'] 
            return  (name1,name2,'temp')
        elif(self.gettop()=='213' or self.gettop()=='221'):
            return  ('_','_',name1)
        else:
            print("error! 期待操作符或者;")
    def OB(self,name = 'null',attri = 'null'):
        if(self.gettop()=='301' or self.gettop()=='401'):
            assin = self.gettopname()
            if(name != 'null'):
                if(self.lookup(assin)):
                    if(attri == table[assin][0]):
                        table[name][1] = table[assin][1]
                    else:
                        print("赋值双方对象类型不匹配！OB")
                        exit(0)    
                else:
                    print("赋值变量不存在 OB")
                    print(assin)
                    exit(0)    
            self.pop()                 
            return assin         
        else:
            print("error,期望'var'或'const' OB()")
            exit(0)
    def OP(self):
        if(self.gettop()=='201' or self.gettop()=='202' or self.gettop()=='203'or self.gettop()=='204'or self.gettop()=='209'):
            operator = self.gettopname()
            self.pop()
            return operator
        else:
            print("error,缺省算术运算符 OP()")
            exit(0)
    def RE_S(self):
        if(self.gettop()=='120'):
            self.pop()
            self.OB()
        else:
            print("error,缺省'return' RE_S()")
            exit(0)  
        print("return 语句") 
    def FU_S(self):
        if(self.gettop()=='301'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                self.PA()
                if(self.gettop()=="213"):
                    self.pop()
                else:
                    print("error,期望')' FU_S")
                    exit(0)
            else:
                print("error,期望'(' FU_S")
                exit(0)                    
        else:
            print("error,函数名错误 FU_S")
            exit(0)
        print("函数调用！")  
    def PA(self):
        if(self.gettop()=='213'):
            pass
        elif(self.gettop()=='301' or self.gettop()=='401'):
            self.OB()
            self.PA_()
        else:
            print("error,(函数调用)参数错误 PA")
            exit(0)
    def PA_(self):
        if(self.gettop()=='213'):
            pass
        elif(self.gettop()=='220'):
            self.pop()
            self.PA()
        else:
            print("error,(函数调用)参数错误 PA_") 
            exit(0)
    def MUL_S(self):
        if(self.gettop()=='216'):
            self.pop()
            self.FUN_B()
            if(self.gettop()=='217'):
                self.pop()
            else:
                print("error,期望'}' MUL_S") 
                exit(0)
            return self.getaddress()        
        else:
            print("error,期望'{' MUL_S")
            exit(0) 
    def CY_S(self):
        if(self.gettop()=='132'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                address = self.CON()
                if(self.gettop()=='213'):
                    self.pop()
                    processaddress = self.MUL_S()
                    self.command[address[1]-1][3] = processaddress
                else:
                    print("error,期望')' CY_S") 
                    exit(0)    
            else:
                print("error,期望'(' CY_S") 
                exit(0)    
        elif(self.gettop()=='108'):
            self.pop()
            topaddress = self.getaddress()
            processaddress = self.MUL_S()
            if(self.gettop()=='132'):
                self.pop()
                if(self.gettop()=='212'):
                    self.pop()
                    address = self.CON()
                    if(self.gettop()=='213'):
                        self.pop()
                        self.command[address[1]-2][3] = topaddress
                        self.command[address[1]-1][3] = processaddress+2 
                    else:
                        print("error,期望')' CY_S") 
                        exit(0)    
                else:
                    print("error,期望'(' CY_S") 
                    exit(0)    
            else:
                print("error") 
                exit(0)    
        elif(self.gettop()=='114'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                self.FOR1()
                topaddress = self.getaddress()
                address = self.CON()
                conditionpart = self.command[address[0]:address[1]]
                if(self.gettop()=='221'):
                    self.pop()
                    for3begin = self.FOR3()
                    for3end   = self.getaddress()
                    if(for3begin != 'null'):
                        for3part = self.command[for3begin:for3end]
                    else:
                        for3part = []    
                    if(self.gettop()=='213'):
                        self.pop()
                        mul_begin = self.getaddress()
                        mul_end = self.MUL_S()
                        mul_part = self.command[mul_begin:mul_end]
                        self.command = self.command[0:address[0]]
                        truestart = self.getaddress()
                        self.command = self.command + mul_part + for3part + conditionpart
                        self.command[self.getaddress()-1][3] = self.getaddress()
                        self.command[self.getaddress()-2][3] = truestart 
                    else:
                        print("error,期望')' CY_S") 
                        exit(0)   
                else:
                    print("error,期望';' CY_S") 
                    exit(0)    
            else:
                print("error,期望'(' CY_S") 
                exit(0)
        else:
            print("error,未识别到循环语句开始符 CY_S") 
            exit(0)
        print("循环体！")          
    def FOR1(self):
        if(self.gettop() == '221'):
            pass
        elif(self.gettop()=='301'):
            self.A_S()
        else:
            print("error,for语句初始化部分错误 FOR1") 
            exit(0)
    def FOR3(self):
        if(self.gettop()=='213'):
            return 'null'
        elif(self.gettop()=='301'):
            for3beginaddress = self.getaddress()
            name = self.gettopname()
            self.pop()
            if(self.gettop()=='205'):
                self.pop()
                result = self.OP_S()
                self.command[self.getaddress()] = ['=',result[2],'_',name]
                return for3beginaddress
            else:
                print("error,期望'='") 
                exit(0)    
        else:
            print("error,for语句迭代错误 FOR3") 
            exit(0)  
    def IF_S(self):
        if(self.gettop()=='116'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                address = self.CON()
                if(self.gettop()=='213'):
                    self.pop()
                    processaddress = self.MUL_S()
                    processaddress = self.IF_S_()
                    self.command[address[1]-1][3] = processaddress
                else:
                    print("error,期望')' IF_S") 
                    exit(0)      
            else:
                print("error,期望'(' IF_S") 
                exit(0)      
        else:
            print("error,期望'if' IF_S") 
            exit(0) 
        print("IF 语句！")  
    def IF_S_(self):
        if(self.gettop()=='110'):
            elseaddress = self.getaddress()
            self.pop()
            self.MUL_S()
            return elseaddress
        elif(self.gettop()=='301'or self.gettop()=='114'or self.gettop()=='132'or self.gettop()=='116'or self.gettop()=='217'or self.gettop()=='216'or self.gettop()=='120'or self.gettop()=='108'):
            return self.getaddress()
        else:
            print("error,if语句错误 IF_S_") 
            exit(0)    
    def CON(self):
        if(self.gettop()=='211'):
            self.gettopname()
            self.pop()
            if(self.gettop()=='301'):
                name = self.gettopname()
                self.pop()
                ifaddress = self.getaddress()
                self.command[self.getaddress()] = ['!',name,'_','temp1']
                self.command[self.getaddress()] = ['jud','temp1','_',(self.getaddress()+2)]
                self.command[self.getaddress()] = ['ju','_','_','somewherefalse']
                iffalseaddress = self.getaddress()
                return (ifaddress,iffalseaddress)
            else:
                print("error,期望'var' CON") 
                exit(0)
        elif(self.gettop()=='301' or self.gettop()=='401'):
            name1 = self.OB()
            result = self.CON_(name1)
            return result
        else:
            print("error,条件错误 CON") 
            exit(0)
    def CON_(self,name1):
        if(self.gettop()=='213'):
            ifaddress = self.getaddress()
            self.command[ifaddress] = ['jud',name1,'_',(ifaddress+2)]
            self.command[ifaddress+1] = ['ju','_','_','somewherefalse']
            falseaddress = self.getaddress()
            return (ifaddress,falseaddress)
        elif(self.gettop()=='207' or self.gettop() == '225'or self.gettop()== '208'or self.gettop()=='226'or self.gettop()== '227'):
            operator = self.gettopname()
            self.pop()
            name2 = self.OB()
            ifaddress = self.getaddress()
            self.command[ifaddress] = [operator,name1,name2,(ifaddress+2)]
            self.command[ifaddress+1] = ['ju','_','_','somewherefalse']
            falseaddress = self.getaddress()
            return(ifaddress,falseaddress)
        else:
            print(self.signal[0])
            print("error,条件错误 CON_") 
            exit(0)                                                                       
    def TYPE(self):
        if(self.gettop()=='113'or self.gettop()=='117' or self.gettop()=='109'):
            t = self.gettopname()
            self.pop()
            return t
        else:
            print("error,无此变量类型  TYPE")
            exit(0)      

    


            
