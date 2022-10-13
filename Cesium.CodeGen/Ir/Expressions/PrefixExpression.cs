using Cesium.CodeGen.Contexts;
using Cesium.CodeGen.Extensions;
using Cesium.CodeGen.Ir.Expressions.BinaryOperators;
using Cesium.CodeGen.Ir.Expressions.Constants;
using Cesium.CodeGen.Ir.Types;
using Cesium.Core;
using Yoakke.SynKit.C.Syntax;
using Yoakke.SynKit.Lexer;

namespace Cesium.CodeGen.Ir.Expressions;

internal class PrefixExpression : IExpression
{
    private readonly IExpression _target;
    private readonly BinaryOperator _operator;
    public PrefixExpression(Ast.PrefixExpression expression)
    {
        expression.Deconstruct(out var prefixOperator, out var target);
        _target = target.ToIntermediate();
        _operator = GetOperator(prefixOperator);
    }

    public IExpression Lower(IDeclarationScope scope)
    {
        var target = _target.Lower(scope);
        return new AssignmentExpression(
            target,
            BinaryOperator.Assign,
            new ArithmeticBinaryOperatorExpression(
                target,
                _operator,
                new ConstantLiteralExpression(new IntegerConstant("1"))
            )
        ).Lower(scope);
    }

    public void EmitTo(IEmitScope scope) => throw new AssertException("Should be lowered");

    public IType GetExpressionType(IDeclarationScope scope) => _target.GetExpressionType(scope);

    private static BinaryOperator GetOperator(IToken<CTokenType> token) => token.Kind switch
    {
        CTokenType.Increment => BinaryOperator.Add,
        CTokenType.Decrement => BinaryOperator.Subtract,
        _ => throw new AssertException($"Token type {token.Kind} is invalid"),
    };
}
