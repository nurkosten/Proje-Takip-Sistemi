using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProjeHavuzu.Data.Context;
using ProjeHavuzu.Data.Entites;
using ProjeHavuzu.Data.Entites.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjeHavuzu.Data.Helpers
{
    public static class FacultyDepartmentSeed
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var context = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<ApplicationContext>())
            {
                // Veri varsa seed işlemini atla (Veri bütünlüğünü korumak için silme yapma)
                if (await context.Faculties.AnyAsync())
                {
                    return; 
                }

                // Yeni Verileri Hazırla
                var rawList = new List<FacultyData>
                {
                    new FacultyData("Mühendislik ve Doğa Bilimleri Fakültesi", new[] {
                        "Bilgisayar Mühendisliği", "Elektrik-Elektronik Mühendisliği", "İnşaat Mühendisliği", "Yazılım Mühendisliği"
                    }),
                    new FacultyData("Sağlık Bilimleri Fakültesi", new[] {
                        "Acil Yardım ve Afet Yönetimi", "Beslenme ve Diyetetik", "Ebelik", "Hemşirelik", "Sosyal Hizmet"
                    }),
                    new FacultyData("Sanat, Tasarım ve Mimarlık Fakültesi", new[] {
                        "Görsel İletişim Tasarımı", "İç Mimarlık", "Radyo, Televizyon ve Sinema"
                    }),
                    new FacultyData("Sivil Havacılık Yüksekokulu", new[] {
                        "Havacılık Yönetimi"
                    }),
                    new FacultyData("Sosyal ve Beşeri Bilimler Fakültesi", new[] {
                        "İngilizce Mütercim ve Tercümanlık", "Kültür Varlıklarını Koruma ve Onarım", "Muhasebe ve Finans Yönetimi", "Psikoloji", "Siyaset Bilimi ve Kamu Yönetimi", "Sosyoloji", "Tarih", "Turizm İşletmeciliği", "Türk Dili ve Edebiyatı", "Uluslararası İşletme Yönetimi", "Uluslararası Ticaret ve Finansman", "Yeni Medya ve İletişim", "Yönetim Bilişim Sistemleri"
                    }),
                    new FacultyData("Tıp Fakültesi", new[] {
                        "Tıp"
                    }),
                    new FacultyData("Ziraat Fakültesi", new[] {
                        "Bahçe Bitkileri", "Bitki Koruma", "Biyosistem Mühendisliği", "Su Ürünleri Mühendisliği", "Tarım Ekonomisi", "Tarla Bitkileri", "Toprak Bilimi ve Bitki Besleme", "Zootekni"
                    })
                };

                // Manuel Sıraya Göre Ekleme (Kullanıcının verdiği liste sırası)
                // Sıralamayı korumak için CreatedDate'i manipüle ediyoruz.
                var baseDate = DateTime.UtcNow;
                int orderIndex = 0;

                foreach (var fData in rawList)
                {
                    orderIndex++;
                    var facultyDate = baseDate.AddSeconds(orderIndex);

                    var faculty = new Faculty
                    {
                        Id = Guid.NewGuid(),
                        FacultyName = fData.Name,
                        Status = DataStatus.Inserted,
                        CreatedDate = facultyDate
                    };

                    await context.Faculties.AddAsync(faculty);

                    foreach (var dName in fData.Departments)
                    {
                        orderIndex++;
                        var departmentDate = baseDate.AddSeconds(orderIndex);

                        var department = new Department
                        {
                            Id = Guid.NewGuid(),
                            DepartmentName = dName,
                            FacultyId = faculty.Id,
                            Status = DataStatus.Inserted,
                            CreatedDate = departmentDate
                        };
                        await context.Departments.AddAsync(department);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        private class FacultyData
        {
            public string Name { get; }
            public string[] Departments { get; }
            public FacultyData(string name, string[] departments)
            {
                Name = name;
                Departments = departments;
            }
        }
    }
}
