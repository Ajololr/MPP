using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ClassLibrary1
{
    public class FakerConfig
    {
        public Dictionary<Type, Dictionary<Expression, Type>> ClassesDictionary = new Dictionary<Type, Dictionary<Expression, Type>>();

        public void Add<C, T, G>(Expression<Func<C, T>> FieldGetter) where C: class
        {
            var temp = new Dictionary<Expression, Type>();
            temp.Add(FieldGetter, typeof(G));
            
            ClassesDictionary.Add(typeof(C), temp);
        }
        
        
    }
}