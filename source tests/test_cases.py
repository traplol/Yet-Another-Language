# demonstrates a simple function 
func a(a,b){
    return a + b;
}

# demonstrates a for loop with dynamic return type
func b(c){
    for (var i = c; i < 10; i = i + 1;) {
        if (i == 5) {
            return 5;
        }
    }
    return false;
}

# simple while loop
func c(d){
    while(d < 5) {
        d = d+1;
    }
    return d;
}

# recursive fibonacci
func fibR(x) {
    if (x < 3) {
        return 1;
    } else {
        return fibR(x-1) + fibR(x-2);
    }
}

# iterative factorial
func factorialI(n) {
    var sum = 1;
    if (n <= 1) {
        return sum;
    }
    while (n > 1) {
        sum = sum * n;
        n = n - 1;
    }
    return sum;
}


def a = 5; # immutable
var x = c(1); # variable
x = x * x;

# Run the test cases.
print(a(a,3));
print(b(1));
print(b(10));
print(c(x));
print(fibR(10));
print(factorialI(a));
