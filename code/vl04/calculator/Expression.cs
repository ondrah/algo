using System;
using System.Collections.Generic;

namespace WindowsFormsApp1
{
    /// <summary>
    /// Class for evaluation of arithmetic expressions.
    ///
    /// Note that this class does not allow concurrency and
    /// there can be only one instance used at a time since
    /// the class uses a static buffer variable letters.
    /// </summary>
    class Expression
    {
        /// <summary>
        /// Global array of expression broken down to single characters.
        /// </summary>
        static char[] letters;

        /// <summary>
        /// Evaluate a string as an arithmetic expression.
        /// </summary>
        /// <param name="input"></param>
        public Expression(string input)
        {
            letters = input.ToCharArray();
            eval(0, letters.Length - 1);
        }

        /// <summary>
        /// Evaluate a subexpression specified by index boundaries.
        /// </summary>
        /// <param name="left">Leftmost character index.</param>
        /// <param name="right">Rightmost character index.</param>
        public Expression(int left, int right)
        {
            eval(left, right);
        }

        /// <summary>
        /// Create a simple numeric expression. Used to translate "-(expression)" to "(-1)*(expression)".
        /// </summary>
        /// <param name="_number"></param>
        public Expression(double _number)
        {
            operation = 'n';
            number = _number;
        }

        /// <summary>
        /// left and right part of a binary operation
        /// </summary>
        Expression left, right;
        /// <summary>
        /// 'n' for a single number, arithmetic operation otherwise
        /// </summary>
        char operation;
        /// <summary>
        /// valid if operation set to 'n'
        /// </summary>
        double number;

        /// <summary>
        /// Evaluate an expression or its part. This is a recursive function.
        /// </summary>
        /// <param name="beg">Start index</param>
        /// <param name="end">End index</param>
        /// <returns></returns>
        private void eval(int beg, int end)
        {
            RESTART_EVAL:
            
            // empty expression
            if (beg > end)
                return;

            // negative sign
            bool flNegOp1 = false;
            int nrToken = 0;
            int begToken0 = -1, endToken0 = -1;
            
            // list of indexes of operation at the topmost level
            List<int> operations = new List<int>();
            string strFirstNumber = "";

            for (int i = beg; i <= end; i++)
            {
                if (char.IsWhiteSpace(letters[i]))
                    continue;

                // check for leading + or -
                if(nrToken == 0)
                {
                    switch (letters[i])
                    {
                        // + and - can be unary
                        case '+':
                        case '-':
                            if (nrToken == 0)
                            {
                                flNegOp1 = true;
                                break;
                            }
                            break;
                    }
                }

                // every uneven word can be an operation
                if (nrToken % 2 == 1)
                {
                    switch (letters[i])
                    {
                        case '+':
                        case '-':
                        case '*':
                        case '/':
                            operations.Add(i);
                            nrToken++;
                            break;
                    }
                }
                else
                {   // every even expression can be a number or an expression in parentheses
                    switch (letters[i])
                    {
                        // find matching counterpart ')'
                        case '(':
                            i++;    // skip the opening brace

                            if(nrToken == 0)
                            {
                                begToken0 = i;
                            }

                            int start = i;
                            int braceLevel = 1;
                            while (i <= end)
                            {
                                // NOTE: difficult to use switch here, 'break' used to terminate the loop
                                if (letters[i] == '(')
                                {
                                    braceLevel++;
                                }
                                else if (letters[i] == ')')
                                {
                                    braceLevel--;
                                    if (braceLevel == 0)
                                    {
                                        if(nrToken == 0)
                                        {
                                            endToken0 = i - 1;
                                        }
                                        break;  /* break the while loop */
                                    }
                                }

                                i++;
                            }
                            nrToken++;
                            break;

                        default:
                            if (char.IsDigit(letters[i]))
                            {
                                while (i <= end && (char.IsDigit(letters[i]) || letters[i] == '.' || letters[i] == ','))
                                {
                                    if(nrToken == 0)
                                    {
                                        strFirstNumber += letters[i].ToString();
                                    }
                                    i++;
                                }

                                i--;    // get one back, increased by for
                                nrToken++;
                                break;
                            }
                            
                            // invalid character
                            return;
                    }
                }
            }

            if(operations.Count == 0)
            {
                if(begToken0 > 0)
                {
                    // convert -(expression) to (-1) * (expresion)
                    if(flNegOp1)
                    {
                        operation = '*';
                        left = new Expression(-1);
                        right = new Expression(begToken0, endToken0);
                        return;
                    }

                    // but do not do this with (+1), restart scanning instead
                    beg = begToken0;
                    end = endToken0;
                    goto RESTART_EVAL;
                }

                // parse the number
                if (!double.TryParse(strFirstNumber, out number))
                    return;

                operation = 'n';
                if (flNegOp1)
                {   // apply the sign
                    number = -number;
                }
                return;
            }

           // at least one operation found, start with divisions and multiplications
            int preferredIndex = -1;
            operations.Reverse();
            foreach (int op_index in operations)
            {
                if(letters[op_index] == '+' || letters[op_index] == '-')
                {
                    preferredIndex = op_index;
                    break;
                }
            }

            if(preferredIndex == -1)
            {
                preferredIndex = operations[0];
            }

            operation = letters[preferredIndex];
            left = new Expression(beg, preferredIndex - 1);
            right = new Expression(preferredIndex + 1, end);
        }

        /// <summary>
        /// Compute the result of an expression. This is a recursive function.
        /// </summary>
        /// <returns></returns>
        public double GetResult()
        {
            switch(operation)
            {
                case 'n': return number;
                case '+': return left.GetResult() + right.GetResult();
                case '-': return left.GetResult() - right.GetResult();
                case '*': return left.GetResult() * right.GetResult();
                case '/': return left.GetResult() / right.GetResult();
            }

            throw new Exception($"Invalid operation '{operation.ToString()}'.");
        }

        /// <summary>
        /// Get Reverse Polish Notation of the expression.
        /// </summary>
        /// <returns></returns>
        public string GetRPN()
        {
            switch (operation)
            {
                case 'n': return number.ToString();
                case '+': 
                case '-':
                case '*':
                case '/':
                    return $"{left.GetRPN()}, {right.GetRPN()}, {operation.ToString()}";
            }

            throw new Exception($"Invalid expression '{operation.ToString()}'.");
        }
    }
}
