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
        public static Func<T[], T[], T> GetVectorMultiplyFunction<T>() where T : struct
        {
            var result = Expression.Parameter(typeof(T));
            var iteration = Expression.Parameter(typeof(int));
            var label = Expression.Label(typeof(T));
            ParameterExpression[] args =
            {
                Expression.Parameter(typeof(T[])),
                Expression.Parameter(typeof(T[]))
            };

            BlockExpression block = Expression.Block(
                new[] { result, iteration },
                    Expression.Assign(iteration, Expression.Constant(-1)),
                    Expression.Loop(
                        Expression.IfThenElse(Expression.GreaterThan(Expression.ArrayLength(args[0]), Expression.PreIncrementAssign(iteration)),
                                              Expression.AddAssign(result, Expression.Multiply(Expression.ArrayAccess(args[0], iteration),
                                                                                               Expression.ArrayAccess(args[1], iteration))),
                                              Expression.Break(label, result)
                        ),
                    label)
            );

            return Expression.Lambda<Func<T[], T[], T>>(block, args[0], args[1]).Compile();
        }

        // Static solution to check performance benchmarks
        public static int MultuplyVectors(int[] first, int[] second)
        {
            int result = 0;
            for (int i = 0; i < first.Length; i++)
            {
                result += first[i] * second[i];
            }
            return result;
        }
    }
}
