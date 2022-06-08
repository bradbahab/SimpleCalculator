/*
 * Author: Brad Bahab
 * Date: June 08, 2022
 * Program: Simple Calulator
 * Description: This program will evaluate an input string which is accepted and evaluated as an s-expression.
 *              The evaulation will then be printed to the console.
 */

using System;
using System.Collections.Generic;

namespace SimpleCalculator
{
    class Program
    {
        /*
         * Code clarity: how easy is it to read and reason about your code? 
         * Is data and control flow obvious and easy to follow?
         *
         * Answer: code is separated in to methods where ever possible/neccessary
         * 

         * Abstraction: there are many similarities between the subproblems. 
         * Do you exploit the patterns and have clear delegation of responsibility, or merely copy/paste code?
         * 
         * Answer: all neccessary code has been documented, all code has been written by myself
         
         * User experience: we've explicitly avoided requiring error handling, but how would your code need to be modified if the user provided malformed expressions.
         * How good could you make your error messages?
         * 
         * Answer: the entire process is process is surrounded by a try-catch block
         * Each of the methods created have their own error handling where ever neccessary. 
         * The created methods do not need their own try-catch block as it defined in the parent scope
         * 
         */

        static void Main(string[] args)
        {
            // user input variable
            string input = "";

            /*
             * What if we needed to support an arbitrary number of arguments to add and multiply instead of supporting exactly 2, as in (add 1 2 3 4 (multiply 2 3 5))?
             */
            // max number of expressions that can be entered per function
            // this can be changed at any time programmatically
            int maxNumberOfExpressions = 2;

            // if "Exit" is entered into the INPUT field, the console application will close
            while (input != "exit") 
            {
                try
                {
                    Console.Write("Type \"Exit\" to close application. \n\n");
                    Console.Write("INPUT: ");

                    input = Console.ReadLine();
                    input = input.ToLower();

                    // Create a dynamic list of List objects
                    // that will contain a sequence of parenthesis
                    // in order from opening to closing parenthesis
                    List<char> CapturedParenthesis = new List<char>();

                    // Create a dynamic list of List objects
                    // that will contain a sequence of positions of each parenthesis
                    // in order from opening to closing parenthesis.
                    List<int> PositionsOfParenthesis = new List<int>();

                    // Set the variable "numberOfParenthesis" to -1 to for error handling.
                    int numberOfParenthesis = -1; 
                    int value = 0;

                    // assign the variable "input" to "tempInputBuffer" to keep the original state of the variable "input"
                    string tempInputBuffer = input; 
                    string tempSubStr;

                    // continue to loop through the algorithm until the string is entirely replaced with the final value
                    while (numberOfParenthesis != 0) 
                    {
                        numberOfParenthesis = getNumberOfParenthesis(tempInputBuffer);

                        if (evaluateParenthesisAmount(numberOfParenthesis))
                        {
                            // divide the total number of parenethesis by two, this will give the index of the first closing parenthesis in the PositionsOfParenthesis List object
                            // int declaration is set to this scope in order for the memory location to be destroyed and recreated
                            int firstClosingParenthesis = numberOfParenthesis / 2;

                            // subtract the firstClosingParenthesis by 1 to get the index of the last opening parenthesis in the PositionsOfParenthesis List object
                            // int declaration is set to this scope in order for the memory location to be destroyed and recreated
                            int startingIndexOfOpeningParenthesis = firstClosingParenthesis - 1;

                            getNumberOfParenthesis(tempInputBuffer);
                            
                            // char array declaration is set to this scope in order for the memory location to be destroyed and recreated
                            char[] inputArray = tempInputBuffer.ToCharArray();

                            PopulateCapturedParenthesis(CapturedParenthesis, PositionsOfParenthesis, inputArray);

                            if (numberOfParenthesis != 0)
                            {
                                // int declaration is set to this scope in order for the memory location to be destroyed and recreated
                                int length = PositionsOfParenthesis[firstClosingParenthesis] - PositionsOfParenthesis[startingIndexOfOpeningParenthesis];
                                
                                // extract the first "function call" in order to evaluate its sub expression
                                tempSubStr = tempInputBuffer.Substring(PositionsOfParenthesis[startingIndexOfOpeningParenthesis], (length + 1));
                            }
                            else
                            {
                                // prepare tempSubStr in case there are no parenthesis detected
                                tempSubStr = tempInputBuffer.ToLower().Trim();
                            }

                            // return the value from the calulcated function in the variable tempSubStr
                            value = sExpression(tempSubStr, maxNumberOfExpressions);
                            if (value == -1)
                            {
                                Console.WriteLine("ERROR: Each function must only include 2 sub expressions or only 1 base 10 digit." +
                                    "");
                                break;
                            }

                            // replace part of the text in the variable tempInputBuffer with the matching text in tempSubStr with the returned value from the function "sExpression"
                            tempInputBuffer = tempInputBuffer.Replace(tempSubStr, value.ToString());
                            
                            // Clear both of the List objects so that they can be used to populate the new value of the variable tempInputBuffer
                            CapturedParenthesis.Clear();
                            PositionsOfParenthesis.Clear();
                        }
                        else
                        {
                            Console.WriteLine("ERROR: Missing equal number of parenthesis");
                            input = "";
                            tempInputBuffer = "";
                            value = -1;
                            break;
                        }
                    }
                    if (value != -1)
                    {
                        Console.WriteLine("OUTPUT: " + value);
                    }
                    
                    Console.WriteLine("\nPress Enter to restart.");
                    Console.ReadLine();
                    Console.Clear();
                }
                catch (Exception e)
                {
                    Console.WriteLine("ERROR: " + e);
                }
            }

        }


        #region   -----------METHODS------------


        /// <summary>
        /// PopulateCapturedParenthesis will traverse through the index of the char array
        /// and will populate two List objects with all the detected parenthesis and their positions
        /// based on the index of the char array
        /// </summary>
        /// <param name="capturedParenthesis"></param>
        /// <param name="positionOfParenthesis"></param>
        /// <param name="inputArray"></param>
        private static void PopulateCapturedParenthesis(List<char> capturedParenthesis, List<int> positionOfParenthesis, char[] inputArray)
        {
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i] == '(')
                {
                    capturedParenthesis.Add(inputArray[i]);
                    positionOfParenthesis.Add(i);
                }
                else if (inputArray[i] == ')')
                {
                    capturedParenthesis.Add(inputArray[i]);
                    positionOfParenthesis.Add(i);
                }
            }
        }

        /// <summary>
        /// evaluateParenthesisAmount will divide the numberOfParenthesis with remainder operator.
        /// If the remainder equals 0, then the amount of parenethis are considered to be adequate. 
        /// </summary>
        /// <param name="numberOfParenthesis"></param>
        /// <returns>evaluation bool</returns>
        private static bool evaluateParenthesisAmount(int numberOfParenthesis)
        {
            bool evaluation;
            int remainder = numberOfParenthesis % 2;

            if (remainder == 0)
            {
                evaluation = true;
            }
            else
            {
                evaluation = false;
            }
            return evaluation;
        }

        /// <summary>
        /// getNumberOfParenthesis will traverse through a char array to detect either a '(' or ')' char.
        /// Once a '(' or ')' char is detected, then the counter is incremented and is returned
        /// </summary>
        /// <param name="input"></param>
        /// <returns>counter int</returns>
        private static int getNumberOfParenthesis(string input)
        {
            int counter = 0;
            char[] inputArray = input.ToCharArray();
            for (int i = 0; i < inputArray.Length; i++)
            {
                if (inputArray[i] == '(')
                {
                    counter++;
                }
                if (inputArray[i] == ')')
                {
                    counter++;
                }
            }
            return counter;
        }


        /// <summary>
        /// sExpression will calculate the isolated function either by adding, multiplying, or accepting a single base 10 digit.
        /// If the add or multiply function is called, the number sub expressions will be counted and confirmed by the "maxNumberOfExpressions" value
        /// The final result will be returned.
        /// If an error is detected, -1 will be returned
        /// </summary>
        /// <param name="invocation"></param>
        /// <param name="maxNumberOfExpressions"></param>
        /// <returns>currentValueTemp int</returns>
        private static int sExpression(string invocation, int maxNumberOfExpressions)
        {
            int value = 0;
            int currentValueTemp = 0;
            string error;
            int pos;
            string[] invocationValues;

            // detect the number of expressions in the funnction call
            invocationValues = invocation.Split();

            if (invocationValues.Length > 1)
            {
                // find the positions of each '('  and ')' character and remove them from the string "invocation"
                pos = invocation.IndexOf('(');
                invocation = invocation.Remove(pos, 1);
                pos = invocation.IndexOf(')');
                invocation = invocation.Remove(pos, 1);
            }
            // Redundent sanity check after removal of the parenthesis: detect the number of expressions in the funnction call
            invocationValues = invocation.Split();

            // maxNumberOfExpressions must have 1 added to it in order to match the entire array length
            // including the function call of either "add" or "multiply" plus the expressions
            if (invocationValues.Length == (maxNumberOfExpressions + 1))
            {
                // the first index of the array should be either "add" or "multiply"
                // otherwise the default switch case will be performed which produces an error message
                switch (invocationValues[0].ToLower().Trim())
                {
                    case "add":
                        for (int i = 1; i < invocationValues.Length; i++)
                        {
                            try
                            {
                                // Parse the string value to an int data type
                                int tempInteger = Int32.Parse(invocationValues[i]);
                                currentValueTemp += (value + tempInteger);
                            }
                            catch (Exception)
                            {
                                error = "ERROR: " + invocationValues[i] + " is not a base 10 number. Please try again.";
                                currentValueTemp = -1;
                                Console.WriteLine(error);
                                Console.ReadLine();
                            }
                        }
                        break;

                    case "multiply":
                        for (int i = 1; i < invocationValues.Length; i++)
                        {
                            int tempIncrementalIndex = i + 1;
                            if (tempIncrementalIndex < invocationValues.Length)
                            {
                                try
                                {
                                    // Parse the string value to an int data type
                                    int multiplier = Int32.Parse(invocationValues[i]);
                                    int multiplicand = Int32.Parse(invocationValues[tempIncrementalIndex]);
                                    
                                    // if the value is being evaluated for first time in this iteration
                                    if (currentValueTemp == 0)
                                    {
                                        currentValueTemp = (multiplier * multiplicand);
                                    }
                                    else
                                    {
                                        // this else statement will only be triggered if the max number of expressions is increased more than 2 by logical default
                                        currentValueTemp *= multiplicand;
                                    }

                                }
                                catch (Exception)
                                {
                                    error = "ERROR: " + invocationValues[i] + " is not a base 10 number. Please try again.";
                                    Console.WriteLine();
                                    Console.WriteLine(error);
                                    Console.ReadLine();
                                }
                            }
                        }
                        break;


                    /*
                     * What if we needed to add another function type, like (exponent 2 5) that calculates 2^5 = 32? Does that have a natural place to fit into your code or would that require large scale reworking?
                     */
                    // implemeneted as an example of scalabilty
                    case "exponent":
                        for (int i = 1; i < invocationValues.Length; i++)
                        {
                            int tempIncrementalIndex = i + 1;
                            if (tempIncrementalIndex < invocationValues.Length)
                            {
                                try
                                {
                                    // Parse the string value to a double data type
                                    double baseNumber = Double.Parse(invocationValues[i]);
                                    double exponent = Double.Parse(invocationValues[tempIncrementalIndex]);

                                    // if the value is being evaluated for first time in this iteration
                                    if (currentValueTemp == 0)
                                    {
                                        double tempDbl = Math.Pow(baseNumber, exponent);
                                        currentValueTemp = (int)tempDbl;
                                    }
                                    else
                                    {
                                        // this else statement will only be triggered if the max number of expressions is increased more than 2 by logical default
                                        double tempDbl = Math.Pow(currentValueTemp, exponent);
                                        currentValueTemp = currentValueTemp = (int)tempDbl;
                                    }

                                }
                                catch (Exception)
                                {
                                    error = "ERROR: " + invocationValues[i] + " is not a base 10 number. Please try again.";
                                    Console.WriteLine();
                                    Console.WriteLine(error);
                                    Console.ReadLine();
                                }
                            }
                        }
                        break;

                    default:
                        Console.WriteLine();
                        Console.WriteLine("ERROR: " + invocationValues[0] + " cannot be accepted as an operator. Please use either \"add\" or \"multiply\".");
                        currentValueTemp = -1;

                        break;
                }
            }
            // this is meant to catch an array length equal to 1, which is assumed to be a base 10 digit
            else if (invocationValues.Length == 1)
            {
                int tempInteger = Int32.Parse(invocationValues[0]);
                currentValueTemp = tempInteger;
            }
            else
            {
                currentValueTemp = -1;
            }
            return currentValueTemp;
        }
        #endregion
    }

}
