#!/bin/bash
# ============================================================================
#  BlogPlatform — Adım adım Git commit scripti
#  Bu scripti projenin KÖK klasöründe (BlogPlatform.sln'in yanında) çalıştır.
# ============================================================================

set -e  # Hata olursa dur

# --- 0. Ön kontrol -----------------------------------------------------------
if [ ! -f "BlogPlatform.sln" ]; then
  echo "❌ HATA: Bu scripti BlogPlatform.sln'in olduğu klasörde çalıştır."
  exit 1
fi

# --- 1. Git repo'yu başlat ---------------------------------------------------
if [ ! -d ".git" ]; then
  git init
  git branch -M main
fi

# (Opsiyonel) Kendi bilgilerini bir kere ayarlamak istersen:
# git config user.name  "Adın Soyadın"
# git config user.email "mail@ornek.com"

# Build artifact'larını her ihtimale karşı temizle (commit'e karışmasın)
find . -type d \( -name "bin" -o -name "obj" \) -exec rm -rf {} + 2>/dev/null || true

# --- 2. Initial commit: .gitignore + solution -------------------------------
git add .gitignore BlogPlatform.sln
git commit -m "chore: initial commit with solution file and gitignore"

# --- 3. Domain katmanı -------------------------------------------------------
git add src/Blog.Domain/
git commit -m "feat(domain): add core entities, exceptions and repository interfaces

- Add BaseEntity, Author, Post, Comment, Tag, PostTag entities
- Add DomainException for domain-level errors
- Add IRepository, IUnitOfWork, IPostRepository, IAuthorRepository
- Add PostSearchCriteria for filtering posts"

# --- 4. Application katmanı --------------------------------------------------
git add src/Blog.Application/
git commit -m "feat(application): add DTOs, services, validators and mappings

- Add Post, Author, Comment DTOs plus generic Result and PagedResult
- Add PostService, AuthorService, CommentService with their interfaces
- Add FluentValidation validators for create/update DTOs
- Add AutoMapper MappingProfile and SlugHelper"

# --- 5. Infrastructure (DbContext + repositories) ----------------------------
git add src/Blog.Infrastructure/Blog.Infrastructure.csproj \
        src/Blog.Infrastructure/Persistence/BlogDbContext.cs \
        src/Blog.Infrastructure/Persistence/BlogDbSeeder.cs \
        src/Blog.Infrastructure/Persistence/Configurations/ \
        src/Blog.Infrastructure/Repositories/
git commit -m "feat(infrastructure): add EF Core DbContext, configurations and repositories

- Add BlogDbContext and BlogDbSeeder
- Add EF Core entity configurations for Author, Post, Comment, Tag, PostTag
- Add generic Repository plus PostRepository and AuthorRepository"

# --- 6. Infrastructure (Migrations) -----------------------------------------
git add src/Blog.Infrastructure/Persistence/Migrations/
git commit -m "feat(infrastructure): add initial EF Core migration"

# --- 7. Api katmanı ----------------------------------------------------------
git add src/Blog.Api/
git commit -m "feat(api): add controllers, exception middleware and DI setup

- Add PostsController and AuthorsController
- Add ExceptionMiddleware for global error handling
- Configure DI, AutoMapper, FluentValidation, Swagger in Program.cs
- Add appsettings and launchSettings"

# --- 8. Testler --------------------------------------------------------------
git add tests/
git commit -m "test: add unit and integration tests

- Add PostServiceTests (unit)
- Add SlugHelperTests (unit)
- Add PostsApiTests with CustomWebApplicationFactory (integration)"

# --- 9. README (eğer dosya varsa) -------------------------------------------
if [ -f "README.md" ]; then
  git add README.md
  git commit -m "docs: add project README"
fi

# --- 10. Kalan her şey (varsa) ----------------------------------------------
if [ -n "$(git status --porcelain)" ]; then
  git add .
  git commit -m "chore: add remaining project files"
fi

echo ""
echo "✅ Tüm commit'ler atıldı. Geçmişi görmek için:"
echo "   git log --oneline"
echo ""
echo "🚀 Şimdi GitHub'a göndermek için:"
echo "   git remote add origin https://github.com/KULLANICI_ADIN/BlogPlatform.git"
echo "   git push -u origin main"
