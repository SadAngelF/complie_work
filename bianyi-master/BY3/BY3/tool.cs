using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace BY3
{
    //产生式类
    class Product
    {
        String left;//左部
        ArrayList right;//右部
        ArrayList first;//first集
        ArrayList follow;//follow集
        ArrayList select;//select集
        public Product(String left, ArrayList right)
        {
            this.left = left;
            this.right = right;
            first = new ArrayList();
            follow = new ArrayList();
            select = new ArrayList();
        }
        public String getLeft()
        {
            return left;
        }
        public ArrayList getRight()
        {
            return right;
        }
        public void addSelect(ArrayList s)
        {
            foreach (Object o in s)
            {
                if (!this.select.Contains(o))
                {
                    this.select.Add(o);
                }
            }
        }
        public ArrayList getSelect()
        {
            return select;
        }
    }
    class WFormat
    {
        //格式化终结符与非终结符
        public static String name(WordCode2 w)
        {
            if (w.getId() < 300)
            {
                return w.getName();
            }
            else
            {
                return "<" + w.getName() + ">";
            }
        }
    }
}
