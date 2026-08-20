using FluentValidation;
using MovieAPI.DTOs;
namespace MovieAPI.Validators
{
    public class GetMoviesQueryValidator : AbstractValidator<GetMoviesQueryDTO>
    {
        public GetMoviesQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThanOrEqualTo(1).WithMessage("Sayfa numarası en az 1 olabilir.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100).WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) || new[] { "vote", "votecount", "date" }.Contains(sortBy.ToLower()))
            .WithMessage("Sadece 'vote', 'votecount' veya 'date' kriterlerine göre sıralama yapabilirsiniz.");
        }
    }
}
