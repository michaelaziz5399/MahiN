using AutoMapper;
using AutoMapper.QueryableExtensions;
using MahiN.DataBase.Data;
using MahiN.DataBase.Dtos.PlaygroundDtos;
using MahiN.DataBase.Models;
using MahiN.Services.FileService;
using Microsoft.EntityFrameworkCore;

namespace MahiN.Services.PlaygroundService
{
    public class PlaygroundRepository(ApplicationDbContext _context 
        ,IMapper _mapper
        ,IFileService _fileService) : IPlaygroundRepository
    {
        private const string PlaygroundsFolder = "images/playgrounds";
        public async Task<IEnumerable<PlaygroundResponseDto>> GetAllPlaygroundsAsync(bool onlyActive = false)
        {
            var query = _context.Playgrounds.AsNoTracking();
            if(onlyActive)
            {
                query = query.Where(p => p.IsActive);
            }

            return await query
                .ProjectTo<PlaygroundResponseDto>(_mapper.ConfigurationProvider)
                .ToListAsync();
        }

        public async Task<PlaygroundResponseDto?> GetPlaygroundByIdAsync(int id)
        {
            return await _context.Playgrounds
                .AsNoTracking()
                .Where(p => p.Id == id)
                .ProjectTo<PlaygroundResponseDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();
        }

        public async Task<PlaygroundResponseDto> CreatePlaygroundAsync(CreatePlaygroundDto model)
        {
            var playground = _mapper.Map<Playground>(model);
            if(model.Photo != null)
            {
                var photoUrl = await _fileService.SaveFileAsync(model.Photo, PlaygroundsFolder);
                playground.PhotoUrl = photoUrl;
            }
            
            _context.Playgrounds.Add(playground);
            await _context.SaveChangesAsync();

            return _mapper.Map<PlaygroundResponseDto>(playground);
        }

        public async Task<PlaygroundResponseDto?> UpdatePlaygroundAsync(int id, CreatePlaygroundDto model)
        {
            var playground = await _context.Playgrounds.FindAsync(id);
            if (playground == null)
            {
                return null;
            }

            // 1. تحديث القيم جوه الـ Entity الموجودة مباشرة من الـ DTO بـ AutoMapper
            _mapper.Map(model, playground);

            // 2. معالجة تحديث الصورة (لو اترفع صورة جديدة)
            if (model.Photo != null)
            {
                // مسح الصورة القديمة من المجلد لو كانت موجودة
                if (!string.IsNullOrEmpty(playground.PhotoUrl))
                {
                    _fileService.DeleteFile(playground.PhotoUrl , PlaygroundsFolder);
                }

                // حفظ الصورة الجديدة
                playground.PhotoUrl = await _fileService.SaveFileAsync(model.Photo, PlaygroundsFolder);
            }
            await _context.SaveChangesAsync();
            return _mapper.Map<PlaygroundResponseDto>(playground);
        }

        public async Task<bool> DeletePlaygroundAsync(int id)
        {
            var playground = await _context.Playgrounds.FindAsync(id);
            if (playground == null)
            {
                return false;
            }

            _context.Playgrounds.Remove(playground);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ToggleStatusAsync(int id)
        {
            var playground = await _context.Playgrounds.FindAsync(id);
            if (playground == null)
            {
                return false;
            }

            playground.IsActive = !playground.IsActive;
            await _context.SaveChangesAsync();
            return true;
        }

    }
}
