using CarShowroom.Application.Common.Extensions;
using CarShowroom.Application.Common.Interfaces;
using CarShowroom.Application.Common.Localization;
using CarShowroom.Application.Common.Models;
using CarShowroom.Application.Features.Branches.Dtos;
using CarShowroom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarShowroom.Application.Features.Branches;

public class BranchService(IApplicationDbContext context) : IBranchService
{
    public async Task<CustomApiResponse<PagedListModel<BranchDto>>> GetBranchesAsync(
        QueryParameters query, CancellationToken cancellationToken = default)
    {
        var source = context.Branches.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim().ToLower();
            source = source.Where(b => b.Name.ToLower().Contains(search)
                || b.Address.ToLower().Contains(search)
                || b.Mobile.Contains(search));
        }

        var paged = await source.ToPagedListAsync(
            b => new BranchDto(b.Id, b.Name, b.Address, b.Mobile, b.CreatedDate),
            query,
            nameof(Branch.Id),
            cancellationToken);

        return CustomApiResponse<PagedListModel<BranchDto>>.Success(paged);
    }

    public async Task<CustomApiResponse<BranchDto>> GetBranchByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var branch = await context.Branches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (branch is null)
        {
            return CustomApiResponse<BranchDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        return CustomApiResponse<BranchDto>.Success(new BranchDto(
            branch.Id, branch.Name, branch.Address, branch.Mobile, branch.CreatedDate));
    }

    public async Task<CustomApiResponse<BranchDto>> CreateBranchAsync(
        CreateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var branch = new Branch
        {
            Name = request.Name.Trim(),
            Address = request.Address.Trim(),
            Mobile = request.Mobile.Trim()
        };

        await context.Branches.AddAsync(branch, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        var dto = new BranchDto(branch.Id, branch.Name, branch.Address, branch.Mobile, branch.CreatedDate);
        return CustomApiResponse<BranchDto>.Created(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<BranchDto>> UpdateBranchAsync(
        int id, UpdateBranchRequest request, CancellationToken cancellationToken = default)
    {
        var branch = await context.Branches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (branch is null)
        {
            return CustomApiResponse<BranchDto>.NotFound(Msg.NotFound(Msg.Branch));
        }

        branch.Name = request.Name.Trim();
        branch.Address = request.Address.Trim();
        branch.Mobile = request.Mobile.Trim();

        await context.SaveChangesAsync(cancellationToken);

        var dto = new BranchDto(branch.Id, branch.Name, branch.Address, branch.Mobile, branch.CreatedDate);
        return CustomApiResponse<BranchDto>.Success(dto, Msg.Saved);
    }

    public async Task<CustomApiResponse<Nothing>> DeleteBranchAsync(int id, CancellationToken cancellationToken = default)
    {
        var branch = await context.Branches.FirstOrDefaultAsync(b => b.Id == id, cancellationToken);
        if (branch is null)
        {
            return CustomApiResponse<Nothing>.NotFound(Msg.NotFound(Msg.Branch));
        }

        // Check if there are cars assigned to this branch
        var hasCars = await context.Cars.AnyAsync(c => c.BranchId == id, cancellationToken);
        if (hasCars)
        {
            return CustomApiResponse<Nothing>.Conflict(Msg.InUse(Msg.Branch));
        }

        context.Branches.Remove(branch);
        await context.SaveChangesAsync(cancellationToken);

        return CustomApiResponse<Nothing>.Success(Nothing.Value, Msg.Deleted);
    }
}
