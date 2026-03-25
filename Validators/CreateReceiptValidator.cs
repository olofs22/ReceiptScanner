using FluentValidation;
using ReceiptProject1.DTOs.ItemDTOs;
using ReceiptProject1.DTOs.ReceiptDTOs;

namespace ReceiptProject1.Validators
{
    public class CreateReceiptValidator : AbstractValidator<CreateReceiptDTO>
    {
        public CreateReceiptValidator()
        {
            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.");

            RuleFor(x => x.StoreName)
                .MaximumLength(100).WithMessage("Store name cannot exceed 100 characters.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("At least one item is required.");

            RuleForEach(x => x.Items).SetValidator(new CreateItemValidator());
        }
    }
    public class CreateItemValidator : AbstractValidator<CreateItemDTO>
    {
        public CreateItemValidator()
        {
            RuleFor(x => x.Title)
                .MaximumLength(100).WithMessage("Item title cannot exceed 100 characters.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price cannot be negative.");

            RuleFor(x => x.Quantity)
                .GreaterThanOrEqualTo(1).WithMessage("Quantity must be at least 1.");
        }
    }
}
