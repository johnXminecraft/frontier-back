using System.ComponentModel.DataAnnotations;

namespace FrontierWeb.Application.DTO
{
    public record PostCreateRequest(
    [Required, StringLength(200)] string Title,
    [Required, StringLength(200)] string Slug,
    [Required] string Content,
    bool Published = true);

    public record PostUpdateRequest(
        [Required, StringLength(200)] string Title,
        [Required, StringLength(200)] string Slug,
        [Required] string Content,
        bool Published = true);

    public record Paged<T>(IEnumerable<T> Items, int Total, int Page, int PageSize);
}
