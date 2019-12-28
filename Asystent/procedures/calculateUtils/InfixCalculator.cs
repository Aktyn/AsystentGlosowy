using System.Numerics;
using System.Text.RegularExpressions;
using System;
namespace Asystent.procedures.calculateUtils
{
    public class InfixCalculator
    {
        /*private static string extractNumber(string str, int start_i) {
            var _out = "";
            var index = start_i;
            
            while( index < str.Length && (str[index] == '.' || str[index] == ',' || Char.IsNumber(str[index]) ) ) 
            {
                _out += str[index];
                index++;
            }
            
            return Regex.Replace(_out, @",", ".");
        }

        private static object ConvertInfixToPostfix(string expression) {
            expression = Regex.Replace(expression, @"[\[{]", "(");
            expression = Regex.Replace(expression, @"[\]}]", ")");
            expression = Regex.Replace(expression, @"\s*", "");
            
            let infix: (string | number | Operation)[] = [];//variables, numbers or operators
            for(let i=0; i<expression.length; i++) {
                let prev_operator: Operation | undefined;
                let found_operator = operators.get(expression[i]);
                let found_func = functions.get( expression.substr(i, 3) );
                
                if( expression[i] === '-' && (i===0 || expression[i-1] === '(' ||
                    ( !!(prev_operator = operators.get(expression[i-1])) && prev_operator.priority < 4 )) )
                {
                    infix.push( <Operation>operators.get('~') );
                }
                else if( found_operator ) {
                    infix.push(found_operator);
                }
                else if( !isNaN(parseInt(expression[i])) ) {
                    let num = extractNumber( expression, i );
                    i += num.length - 1;
                    infix.push( parseFloat(num) );
                }
                else if(found_func) {
                    // console.log('found func:', found_func);
                    infix.push(found_func);
                }
                else
                    infix.push( expression[i] );
            }
            infix.push(')');
            
            //console.log('infix:', infix);
            
            let stack: ('(' | ')' | Operation)[] = ['('];
            let postfix: (number | Operation)[] = [];
            
            let index = 0;
            while( stack.length > 0 ) {
                if( index >= infix.length ) {
                    console.warn('Incorrect infix expression');
                    break;
                }
                
                let element = infix[index];
                if(typeof element === 'number')
                    postfix.push( <number>infix[index] );
                else if(typeof element === 'string') {
                    if(element === '(')
                        stack.push(element);
                    else if(element === ')') {
                        while( stack[stack.length-1] !== '(' )
                            postfix.push( <Operation>stack.pop() );
                        stack.pop();//removes left parenthesis
                    }
                }
                else if(typeof element === 'object') {//Operator
                    let op = <Operation>infix[index];
                    
                    while(typeof stack[stack.length-1] === 'object' && (<Operation>stack[stack.length-1]).priority >= op.priority)
                        postfix.push( <Operation>stack.pop() );
                    
                    stack.push(op);
                }
                
                index++;
            }
            //console.log('postfix:', postfix);
            
            return postfix;
        }

        private static Double CalculatePostfix(postfix: (number | Operation)[]) {
            let stack: (number | bigint)[] = [];
            for(let element of postfix) {
                if( typeof element !== 'object' )//number
                    stack.push( element );
                else {
                    //console.log( element.arguments );
                    
                    //pop last n elements from stack where n = element.arguments
                    let args = stack.splice(stack.length-element.arguments, element.arguments);
                    //let b = <number>stack.pop();
                    //let a = <number>stack.pop();
                    stack.push( element.apply(...args) );
                }
            }
            if(stack.length !== 1) {
                console.error('Incorrect postfix data');
                return NaN;
            }
            return stack[0];
        }


        public static Double Calculate(string expression) {
            return CalculatePostfix( ConvertInfixToPostfix(expression) );
        }*/
    }
}