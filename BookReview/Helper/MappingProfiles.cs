using AutoMapper;
using BookReview.Dto;
using BookReview.Models;

namespace BookReview.Helper
{
    public class MappingProfiles: Profile
    {
        public MappingProfiles() {
            CreateMap<Category, CategoryDto>();
            CreateMap<CategoryDto, Category>();
            CreateMap<CategoryForUpdateDto, Category>();
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>();
            CreateMap<BookForCreateDto, Book>();
            CreateMap<BookForUpdateDto, Book>();   
            CreateMap<ReviewDto, Review>();
            CreateMap<ReviewForUpdateDto, Review>();
            CreateMap<ReviewerDto, Reviewer>();
            CreateMap<ReviewerForUpdateDto, Reviewer>();
            CreateMap<CountryDto, Country>();
            CreateMap<CountryForUpdateDto, Country>();
            CreateMap<Country, CountryDto>();
            CreateMap<Owner, OwnerDto>();
            CreateMap<OwnerDto, Owner>();
            CreateMap<OwnerForUpdateDto, Owner>();
            CreateMap<Review, ReviewDto>();
            CreateMap<Reviewer, ReviewerDto>();
            CreateMap<UserDto, User>();
        }
    }
}
