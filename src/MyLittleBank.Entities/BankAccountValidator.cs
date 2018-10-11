using FluentValidation;
using MyLittleBank.Entities.Properties;

namespace MyLittleBank.Entities
{
    public class BankAccountValidator : AbstractValidator<BankAccount>
    {
        public BankAccountValidator()
        {
            RuleFor(ba => ba.Id)
               .NotNull()
               .WithMessage(string.Format(Resources.message_FieldCannotBeNull, Resources.field_BankAccountId))
               .Equal(0)
               .WithMessage(string.Format(Resources.message_FieldMustBeEqualTo, Resources.field_BankAccountId, 0));

            RuleFor(ba => ba.Balance)
               .NotNull()
               .WithMessage(string.Format(Resources.message_FieldCannotBeNull, Resources.field_BankBalance))
               .GreaterThanOrEqualTo(0)
               .WithMessage(string.Format(Resources.message_FieldMustBeGreaterThanOrEqualTo, Resources.field_BankBalance, 0));
        }
    }
}
