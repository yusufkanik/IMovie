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

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100).WithMessage("Arama metni en fazla 100 karakter olabilir.");

            RuleFor(x => x.MinYear)
                .GreaterThanOrEqualTo(1888).When(x => x.MinYear.HasValue)
                .WithMessage("Yıl en az 1888 olabilir.");

            RuleFor(x => x.MaxYear)
                .GreaterThanOrEqualTo(x => x.MinYear ?? 1888).When(x => x.MaxYear.HasValue && x.MinYear.HasValue)
                .WithMessage("Maksimum yıl, minimum yıldan küçük olamaz.");

            RuleFor(x => x.MinRating)
                .InclusiveBetween(0, 10).When(x => x.MinRating.HasValue)
                .WithMessage("Puan 0 ile 10 arasında olmalıdır.");

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(0, 10).When(x => x.MaxRating.HasValue)
                .WithMessage("Maksimum puan 0 ile 10 arasında olmalıdır.")
                .GreaterThanOrEqualTo(x => x.MinRating!.Value).When(x => x.MaxRating.HasValue && x.MinRating.HasValue)
                .WithMessage("Maksimum puan, minimum puandan küçük olamaz.");

            RuleFor(x => x.SortBy)
                .Must(sortBy => string.IsNullOrEmpty(sortBy) || new[] { "vote", "votecount", "date" }.Contains(sortBy.ToLower()))
                .WithMessage("Sadece 'vote', 'votecount' veya 'date' kriterlerine göre sıralama yapabilirsiniz.");

            RuleFor(x => x.SortOrder)
                .Must(order => string.IsNullOrEmpty(order) || new[] { "asc", "desc" }.Contains(order.ToLower()))
                .WithMessage("Sıralama yönü sadece 'asc' veya 'desc' olabilir.");

            RuleFor(x => x.MinVoteCount)
                .GreaterThanOrEqualTo(0).When(x => x.MinVoteCount.HasValue)
                .WithMessage("Oy sayısı eksi olamaz.");

            RuleForEach(x => x.GenreIds)
                .GreaterThan(0).WithMessage("Geçersiz tür (Genre) ID'si.");
        }
    }
}
