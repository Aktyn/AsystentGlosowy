import notify from "./notifications";

const operators = new Map([
	['+', {
		priority: 1,
		apply: (a, b) => Number(a)+Number(b),
		arguments: 2
	}],
	['-', {
		priority: 1,
		apply: (a, b) => Number(a)-Number(b),
		arguments: 2
	}],
	['*', {
		priority: 2,
		apply: (a, b) => Number(a)*Number(b),
		arguments: 2
	}],
	['/', {
		priority: 2,
		apply: (a, b) => Number(a)/Number(b),
		arguments: 2
	}],
	['~', {//negation
		priority: 3,
		apply: x => -x,
		arguments: 1
	}],
	
	['^', {
		priority: 4,
		apply: (b,p) => Math.pow(b, p),
		arguments: 2
	}],
	
	['!', {
		priority: 5,
		apply: function(n) {//factorial
			let out = window.BigInt(1);
			for(let i=2; i<=n; i++)
				out *= window.BigInt(i);
			return out;
			//return n > 1 ? n*this.apply(n-1) : 1;
		},
		arguments: 1
	}],
]);

//NOTE: functions names should be 3 letters long
const functions = new Map([
	['sin', {
		priority: 6,
		apply: a => Math.sin(a / 180.0 * Math.PI),
		arguments: 1
	}],
	['cos', {
		priority: 6,
		apply: a => Math.cos(a / 180.0 * Math.PI),
		arguments: 1
	}],
	['tan', {
		priority: 6,
		apply: a => Math.tan(a / 180.0 * Math.PI),
		arguments: 1
	}],
]);

/** 
 * @param {string} str
 * @param {number} start_i
 */
function extractNumber(str, start_i) {
	let out = '';
	let index = start_i;
	
    while( index < str.length && (str[index] === '.' || str[index] === ',' || !isNaN(parseInt(str[index])) ) ) 
    {
		out += str[index];
		index++;
	}
	
	return out.replace(/,/g, '.');
}

/** @param {string} expression */
function convertInfixToPostfix(expression) {
	expression = expression.replace(/[\[{]/g, '(')
		.replace(/[\]}]/g, ')').replace(/\s/g, '');
    
    /** @type {string[] | number[] | Operation[]} */
	let infix = [];//variables, numbers or operators
	for(let i=0; i<expression.length; i++) {
        /** @type {Operation} */
		let prev_operator;
		let found_operator = operators.get(expression[i]);
		let found_func = functions.get( expression.substr(i, 3) );
		
		if( expression[i] === '-' && (i===0 || expression[i-1] === '(' ||
			( !!(prev_operator = operators.get(expression[i-1])) && prev_operator.priority < 4 )) )
		{
			infix.push( operators.get('~') );
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
			infix.push(found_func);
		}
		else
			infix.push( expression[i] );
	}
	infix.push(')');
    
    /** @type {Array<'(' | ')' | Operation>} */
    let stack = ['('];
    /** @type {Array<number | Operation>} */
	let postfix = [];
	
	let index = 0;
	while( stack.length > 0 ) {
		if( index >= infix.length ) {
			console.warn('Incorrect infix expression');
			break;
		}
		
		let element = infix[index];
		if(typeof element === 'number')
			postfix.push( infix[index] );
		else if(typeof element === 'string') {
			if(element === '(')
				stack.push(element);
			else if(element === ')') {
				while( stack[stack.length-1] !== '(' )
					postfix.push( stack.pop() );
				stack.pop();//removes left parenthesis
			}
		}
		else if(typeof element === 'object') {//Operator
			let op = infix[index];
			
			while(typeof stack[stack.length-1] === 'object' && (stack[stack.length-1]).priority >= op.priority)
				postfix.push( stack.pop() );
			
			stack.push(op);
		}
		
		index++;
	}
	//console.log('postfix:', postfix);
	
	return postfix;
}

/** @param {number[] | Operation[]} postfix */
function calculatePostfix(postfix) {
    /** @type {number[] | bigint[]} */
	let stack = [];
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

/** @param {string} expression */
export default function calculateInfix(expression) {
    const postfix = calculatePostfix( convertInfixToPostfix(expression) );

    console.log(postfix);


    notify(`${expression} równa się ${postfix}`);
}