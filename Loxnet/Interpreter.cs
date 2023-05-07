namespace Loxnet;
using static Loxnet.TokenType;

public class Interpreter : Expr.Visitor<object>, Stmt.Visitor<object>
{
    private Environment environment = new Environment();

    public void Interpret(List<Stmt> statements)
    {
        try
        {
            foreach (Stmt statement in statements)
            {
                Execute(statement);
            }
        }
        catch (RuntimeError error)
        {
            Lox.RuntimeError(error);
        }
    }
    
    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.value;
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        object right = Evaluate(expr.right);

        switch (expr.op.type)
        {
            case BANG:
                return !IsTruthy(right);
            case MINUS:
                CheckNumberOperand(expr.op, right);
                return -(double) right;
        }

        return null;
    }

    public object VisitVariableExpr(Expr.Variable expr)
    {
        return environment.Get(expr.name);
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.expression);
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        object left = Evaluate(expr.left);
        object right = Evaluate(expr.right);

        switch (expr.op.type)
        {
            case GREATER:
                CheckNumberOperands(expr.op, left, right);
                return (double) left > (double) right;
            case GREATER_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double) left >= (double) right;
            case LESS:
                CheckNumberOperands(expr.op, left, right);
                return (double) left < (double) right;
            case LESS_EQUAL:
                CheckNumberOperands(expr.op, left, right);
                return (double) left <= (double) right;
            case MINUS:
                CheckNumberOperands(expr.op, left, right);
                return (double) left - (double) right;
            case PLUS:
            {
                if (left is Double l && right is Double r)
                {
                    return l + r;
                }
            }
            {
                if (left is string l && right is string r)
                {
                    return l + r;
                }
            }
                throw new RuntimeError(expr.op, "Operands must be two numbers or two strings.");
            case SLASH:
                CheckNumberOperands(expr.op, left, right);
                return (double) left / (double) right;
            case STAR:
                CheckNumberOperands(expr.op, left, right);
                return (double) left * (double) right;
            case BANG_EQUAL:
                return !IsEqual(left, right);
            case EQUAL_EQUAL:
                return IsEqual(left, right);
        }

        return null;
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private void Execute(Stmt stmt)
    {
        stmt.Accept(this);
    }

    public object VisitExpressionStmt(Stmt.Expression stmt)
    {
        Evaluate(stmt.expression);
        return null;
    }

    public object VisitPrintStmt(Stmt.Print stmt)
    {
        object? value = Evaluate(stmt.expression);
        Console.WriteLine(Stringify(value));
        return null;
    }

    public object VisitVarStmt(Stmt.Var stmt)
    {
        object value = null;
        if (stmt.initializer != null)
        {
            value = Evaluate(stmt.initializer);
        }

        environment.Define(stmt.name.lexeme, value);
        return null;
    }

    private bool IsTruthy(object? obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool) obj;
        return true;
    }

    private bool IsEqual(object? a, object? b)
    {
        if (a is null && b is null) return true;
        if (a is null) return false;

        return a.Equals(b);
    }

    private void CheckNumberOperand(Token op, object? operand)
    {
        if (operand is Double) return;
        throw new RuntimeError(op, "Operand must be a number.");
    }

    private void CheckNumberOperands(Token op, object? left, object? right)
    {
        if (left is Double && right is Double) return;

        throw new RuntimeError(op, "Operands must be numbers.");
    }

    public class RuntimeError : Exception
    {
        public readonly Token token;

        public RuntimeError(Token token, string message) : base(message)
        {
            this.token = token;
        }
    }

    private string Stringify(object? obj)
    {
        if (obj is null) return "nil";

        if (obj is Double)
        {
            string text = obj.ToString();
            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;
        }

        return obj.ToString();
    }
}