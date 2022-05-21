using System;
using System.Linq.Expressions;

namespace Reflection.Tasks
{
    public class CodeGeneration
    {
        /// <summary>
        /// Returns the functions that returns vectors' scalar product:
        /// (a1, a2,...,aN) * (b1, b2, ..., bN) = a1*b1 + a2*b2 + ... + aN*bN
        /// Generally, CLR does not allow to implement such a method via generics to have one function for various number types:
        /// int, long, float. double.
        /// But it is possible to generate the method in the run time! 
        /// at: 
        /// http://blogs.msdn.com/b/csharpfaq/archive/2009/09/14/generating-dynamic-methods-with-expression-trees-in-visual-studio-2010.aspx
        /// </summary>
        /// <typeparam name="T">number type (int, long, float etc)</typeparam>
        /// <returns>
        ///   The function that return scalar product of two vectors
        ///   The generated dynamic method should be equal to static MultuplyVectors (see below).   
        /// </returns>
        public static Func<T[], T[], T> GetVectorMultiplyFunction<T>() where T : struct {
            // TODO : Implement GetVectorMultiplyFunction<T>.
            ParameterExpression arg1 = Expression.Parameter(typeof(T[]), "arg");
            ParameterExpression arg2 = Expression.Parameter(typeof(T[]), "arg");

            var method = typeof(CodeGeneration)
                .GetMethod("GenericMultupyVectors")
                .MakeGenericMethod(typeof(T));

            MethodCallExpression methodCall = Expression.Call(
            method,
            arg1,
            arg2);

            return Expression.Lambda<Func<T[], T[], T>>(
            methodCall,
            new ParameterExpression[] { arg1, arg2 }
            ).Compile();
        }

        // Static solution to check performance benchmarks
        public static int MultuplyVectors(int[] first, int[] second) {
            int result = 0;
            for (int i = 0; i < first.Length; i++) {
                result += first[i] * second[i];
            }
            return result;
        }

        public static T GenericMultupyVectors<T>(T[] first, T[] second)
        {
            dynamic result = 0;
            for (int i = 0; i < first.Length; i++)
            {
                result += (dynamic)first[i] * (dynamic)second[i];
            }
            return result;
        }
    }
}
