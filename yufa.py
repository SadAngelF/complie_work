signal = [0,1,2] ##singal为词法分析器输出的列表(逆序入栈)
def gettop():
    return signal[0]
def gettop2():
    return signal[1]
def pop():
    del signal[0]
def S():
    HEAD()
    if(gettop2() != 'main'):
        VS()
    MAIN()
def HEAD():
    if(gettop() == '133'):
        pop()
        if(gettop == '401'):
            pop()
            HEAD()
        else:
            print("error")    
            exit(0)
    elif(gettop()=='float' or gettop()=='double' or gettop()=='int'):
        pass
    else:
        print("error")
        exit(0)
def VS():
    VS_A()
    VS_B()
    if(gettop()=='int' and gettop2()=='main'):
        pass
    elif(gettop()=='int' or gettop()=='float' or gettop()=='double'):
        VS()    
    else:
        print("error")    
        exit(0)
def VS_A():
    TYPE()
    if(gettop == 'var'):
        pop()
    else:
        print("error")    
        exit(0)
def VS_B():
    if(gettop()=='('):
        VS_C()
    else:
        VAR_DE()
def VS_C():
    pop()        
    F_PA()
    if(gettop()==')'):
        pop()
        VS_D()
    else:
        print("error")
        exit(0)
def VS_D():
    if(gettop()==';'):
        pop()
    elif(gettop()=='{'):
        pop()
        FUN_B()
        if(gettop()=='}'):
            pop()
        else:
            print("error")
            exit(0)
    else:
        print("error")
        exit(0)
def FUN_B():
    VAR_DE()
    STATE()
def MAIN():
    if(gettop()=='int'):
        pop()
        if(gettop()=='main'):
            pop()
            if(gettop()=='{'):
                pop()
                FUN_B()
                if(gettop()=='}'):
                    pop()
                else:
                    print("error")
                    exit(0)    
            else:
                print("error")
                exit(0)    
        else:
            print("error")
            exit(0)    
    else:
        print("error")
        exit(0)         
def F_PA():
    TYPE()
    if(gettop()=='var'):
        pop()
        if(gettop()=='('):
            pop()
            F_PA_()
            if(gettop()==')'):
                pop()
            else:
                print("error")
                exit(0) 
        else:
            print("error")
            exit(0)      
    else:
        print("error")
        exit(0) 
def F_PA_():
    if(gettop()==';'):
        pop()
        F_PA()
    elif(gettop()==')'):
        pass
    else:
        print("error")
        exit(0)
def VAR_DE():
    if(gettop()=='int' or gettop()=='float' or gettop()=='double'):
        TYPE()
        if(gettop()=='var'):
            pop()
            VAR_DE_()
            VAR_DE()
        else:
            print("error")
            exit(0)    
    elif(gettop()=='var'or gettop()=='for' or gettop()=='while' or gettop()=='if' or gettop()=='{' or gettop()=='}' or gettop()=='return'):
        pass
    else:
        print("error")
        exit(0)
def VAR_DE_():
    if(gettop()==';'):
        pop()
    elif(gettop()=='='):
        pop()
        OB()
    else:
        print("error")
        exit(0)
def STATE():
    if(gettop()=='}'):
        pass
    else:
        B_STATE()
        STATE()
def B_STATE():
    if(gettop=='var'):
        if(gettop2=='='):
            A_S()
        elif(gettop2=='('):
            FU_S()
        else:
            print("error")
            exit(0)
    elif(gettop()=='while' or gettop()=='do' or gettop()=='for'):
        CY_S()
    elif(gettop()=='return'):
        RE_S()
    elif(gettop()=='if'):
        IF_S()
    elif(gettop()=='{'):
        MUL_S()
    else:
        print("error")
        exit(0)
def A_S():
    if(gettop()=='var'):
        pop()
        if(gettop()=='='):
            pop()
            OP_S()
            if(gettop()==';'):
                pop()
            else:
                print("error")
                exit(0) 
        else:
            print("error")
            exit(0) 
    else:
        print("error")
        exit(0)                
def OP_S():
    OB()
    OP()
    OB()
def OB():
    if(gettop()=='var' or gettop()=='const'):
        pop()                          
    else:
        print("error")
        exit(0)
def OP():
    if(gettop()=='+' or gettop()=='-' or gettop()=='*'or gettop()=='/'or gettop()=='%'):
        pop()
    else:
        print("error")
        exit(0)
def RE_S():
    if(gettop()=='return'):
        pop()
        OB()
    else:
        print("error")
        exit(0)   
def FU_S():
    if(gettop()=='var'):
        pop()
        if(gettop()=='('):
            pop()
            PA()
            if(gettop()==")"):
                pop()
            else:
                print("error")
                exit(0)
        else:
            print("error")
            exit(0)                    
    else:
        print("error")
        exit(0)  
def PA():
    if(gettop()==')'):
        pass
    elif(gettop()=='var' or gettop()=='const'):
        OB()
        PA_()
    else:
        print("error")
        exit(0)
def PA_():
    if(gettop()==')'):
        pass
    elif(gettop()==','):
        pop()
        PA()
    else:
        print("error") 
        exit(0)
def MUL_S():
    if(gettop()=='{'):
        pop()
        FUN_B()
        if(gettop()=='}'):
            pop()
        else:
            print("error") 
            exit(0)
    else:
        print("error")
        exit(0) 
def CY_S():
    if(gettop()=='while'):
        pop()
        if(gettop()=='('):
            pop()
            CON()
            if(gettop()==')'):
                pop()
                MUL_S()
            else:
                print("error") 
                exit(0)    
        else:
            print("error") 
            exit(0)    
    elif(gettop()=='do'):
        pop()
        MUL_S()
        if(gettop()=='while'):
            pop()
            if(gettop()=='('):
                pop()
                CON()
                if(gettop()==')'):
                    pop()
                else:
                    print("error") 
                    exit(0)    
            else:
                print("error") 
                exit(0)    
        else:
            print("error") 
            exit(0)    
    elif(gettop()=='for'):
        pop()
        if(gettop()=='('):
            pop()
            FOR1()
            CON()
            if(gettop()==';'):
                pop()
                FOR3()
                if(gettop()==')'):
                    pop()
                    MUL_S()
                else:
                    print("error") 
                    exit(0)   
            else:
                print("error") 
                exit(0)    
        else:
            print("error") 
            exit(0)
    else:
        print("error") 
        exit(0)          
def FOR1():
    if(gettop==';'):
        pass
    elif(gettop()=='var'):
        A_S()
    else:
        print("error") 
        exit(0)
def FOR3():
    if(gettop()==')'):
        pass
    elif(gettop()=='var'):
        pop()
        if(gettop()=='='):
            pop()
            OP_S()
        else:
            print("error") 
            exit(0)    
    else:
        print("error") 
        exit(0)  
def IF_S():
    if(gettop()=='if'):
        pop()
        if(gettop()=='('):
            pop()
            CON()
            if(gettop()==')'):
                pop()
                MUL_S()
                IF_S_()
            else:
                print("error") 
                exit(0)      
        else:
            print("error") 
            exit(0)      
    else:
        print("error") 
        exit(0)   
def IF_S_():
    if(gettop()=='else'):
        pop()
        MUL_S()
    elif(gettop()=='var'or gettop()=='for'or gettop()=='while'or gettop()=='if'or gettop()=='}'or gettop()=='{'or gettop()=='return'or gettop()=='do'):
        pass
    else:
        print("error") 
        exit(0)    
def CON():
    if(gettop()=='!'):
        pop()
        if(gettop()=='var'):
            pop()
        else:
            print("error") 
            exit(0)
    elif(gettop()=='var' or gettop()=='const'):
        OB()
        CON_()
    else:
        print("error") 
        exit(0)
def CON_():
    if(gettop()==')'):
        pass
    elif(gettop()=='<'):
        pop()
        if(gettop()=='='):
            pop()
            OB()
        else:
            OB()
    elif(gettop()=='>'):
        pop()
        if(gettop()=='='):
            pop()
            OB()
        else:
            OB()
    elif(gettop()=='='):
        pop()
        if(gettop()=='='):
            pop()
            OB()
        else:
            print("error") 
            exit(0)
    else:
        print("error") 
        exit(0)                                                                       
def TYPE():
    if(gettop()=='float'or gettop()=='int' or gettop()=='double'):
        pop()
    else:
        print("error") 
        exit(0)      



 


        
