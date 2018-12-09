class yufa:
    def __init__(self, signal):
        self.signal = signal

    def gettop(self):
        return self.signal[0]
    def gettop2(self):
        return self.signal[1]
    def pop(self):
        del self.signal[0]
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
            print("HEAD!")
        elif(self.gettop()=='113' or self.gettop()=='109' or self.gettop()=='117'):
            pass
        else:
            print("error, HEAD")
            exit(0)
    def VS(self):
        self.VS_A()
        self.VS_B()
        if(self.gettop()=='117' and self.gettop2()=='134'):
            pass
        elif(self.gettop()=='117' or self.gettop()=='113' or self.gettop()=='109'):
            self.VS()    
        else:
            print("error,缺少返回值类型 VS")    
            exit(0)
        print("VS!")
    def VS_A(self):
        self.TYPE()
        if(self.gettop() == '301'):
            self.pop()
        else:
            print("error,变量缺省 VS_A")    
            exit(0)
    def VS_B(self):
        if(self.gettop()=='212'):
            self.VS_C()
        else:
            self.VAR_DE()
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
        print("语句块！")
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
            self.TYPE()
            if(self.gettop()=='301'):
                self.pop()
                self.VAR_DE_()
                self.VAR_DE()
            else:
                print("error,变量定义错误 VAR_DE")
                exit(0)   
            print("变量定义！")
        elif(self.gettop()=='301'or self.gettop()=='114' or self.gettop()=='132' or self.gettop()=='116' or self.gettop()=='216' or self.gettop()=='217' or self.gettop()=='120'):
            pass
        else:
            print("error,变量定义错误 VAR_DE")
            exit(0)
        
    def VAR_DE_(self):
        if(self.gettop()=='221'):
            self.pop()
        elif(self.gettop()=='205'):
            self.pop()
            self.OB()
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
            self.pop()
            if(self.gettop()=='205'):
                self.pop()
                self.OP_S()
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
        self.OB()
        self.OP_S_()
    def OP_S_(self):
        if(self.gettop()=='201' or self.gettop()=='202' or self.gettop()=='203'or self.gettop()=='204'or self.gettop()=='209'):
            self.OP()
            self.OB()
        elif(self.gettop()=='213' or self.gettop()=='221'):
            pass
        else:
            print("error! 期待操作符或者;")
    def OB(self):
        if(self.gettop()=='301' or self.gettop()=='401'):
            self.pop()                          
        else:
            print("error,期望'var'或'const' OB()")
            exit(0)
    def OP(self):
        if(self.gettop()=='201' or self.gettop()=='202' or self.gettop()=='203'or self.gettop()=='204'or self.gettop()=='209'):
            self.pop()
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
        else:
            print("error,期望'{' MUL_S")
            exit(0) 
    def CY_S(self):
        if(self.gettop()=='132'):
            self.pop()
            if(self.gettop()=='212'):
                self.pop()
                self.CON()
                if(self.gettop()=='213'):
                    self.pop()
                    self.MUL_S()
                else:
                    print("error,期望')' CY_S") 
                    exit(0)    
            else:
                print("error,期望'(' CY_S") 
                exit(0)    
        elif(self.gettop()=='108'):
            self.pop()
            self.MUL_S()
            if(self.gettop()=='132'):
                self.pop()
                if(self.gettop()=='212'):
                    self.pop()
                    self.CON()
                    if(self.gettop()=='213'):
                        self.pop()
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
                self.CON()
                if(self.gettop()=='221'):
                    self.pop()
                    self.FOR3()
                    if(self.gettop()=='213'):
                        self.pop()
                        self.MUL_S()
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
            pass
        elif(self.gettop()=='301'):
            self.pop()
            if(self.gettop()=='205'):
                self.pop()
                self.OP_S()
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
                self.CON()
                if(self.gettop()=='213'):
                    self.pop()
                    self.MUL_S()
                    self.IF_S_()
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
            self.pop()
            self.MUL_S()
        elif(self.gettop()=='301'or self.gettop()=='114'or self.gettop()=='132'or self.gettop()=='116'or self.gettop()=='217'or self.gettop()=='216'or self.gettop()=='120'or self.gettop()=='108'):
            pass
        else:
            print("error,if语句错误 IF_S_") 
            exit(0)    
    def CON(self):
        if(self.gettop()=='211'):
            self.pop()
            if(self.gettop()=='301'):
                self.pop()
            else:
                print("error,期望'var' CON") 
                exit(0)
        elif(self.gettop()=='301' or self.gettop()=='401'):
            self.OB()
            self.CON_()
        else:
            print("error,条件错误 CON") 
            exit(0)
    def CON_(self):
        if(self.gettop()=='213'):
            pass
        elif(self.gettop()=='207' or self.gettop() == '225'or self.gettop()== '208'or self.gettop()=='226'or self.gettop()== '227'):
            self.pop()
            self.OB()
        else:
            print(self.signal[0])
            print("error,条件错误 CON_") 
            exit(0)                                                                       
    def TYPE(self):
        if(self.gettop()=='113'or self.gettop()=='117' or self.gettop()=='109'):
            self.pop()
        else:
            print("error,无此变量类型  TYPE")
            exit(0)      

    


            
