'use client';

import { useState, useEffect, Suspense } from 'react';
import { useRouter, useSearchParams, usePathname } from 'next/navigation';
import { movieService, GetMoviesQuery } from '@/services/movieService';
import { Movie } from '@/types/movie';
import { useDebounce } from '@/hooks/useDebounce';

function CatalogContent() {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  // 1. Initial State'leri Doğrudan URL'den Oku
  const [searchTerm, setSearchTerm] = useState(searchParams.get('q') || '');
  const [sortBy, setSortBy] = useState<'vote' | 'votecount' | 'date'>(
    (searchParams.get('sortBy') as any) || 'date'
  );
  const [sortOrder, setSortOrder] = useState<'asc' | 'desc'>(
    (searchParams.get('sortOrder') as any) || 'desc'
  );
  const [useTmdbSearch, setUseTmdbSearch] = useState(searchParams.get('tmdb') === 'true');

  const [minYear, setMinYear] = useState(searchParams.get('minYear') || '');
  const [maxYear, setMaxYear] = useState(searchParams.get('maxYear') || '');
  const [minRating, setMinRating] = useState(searchParams.get('minRating') || '');
  const [maxRating, setMaxRating] = useState(searchParams.get('maxRating') || '');

  const [page, setPage] = useState(Number(searchParams.get('page')) || 1);
  const [totalPages, setTotalPages] = useState(1);
  const [totalRecords, setTotalRecords] = useState(0);

  const [showFilters, setShowFilters] = useState(
    Boolean(minYear || maxYear || minRating || maxRating)
  );

  const [movies, setMovies] = useState<Movie[]>([]);
  const [loading, setLoading] = useState(true);
  const [syncingTmdbId, setSyncingTmdbId] = useState<number | null>(null);

  const debouncedSearch = useDebounce(searchTerm, 400);

  // 2. State Değiştiğinde URL'i Güncelle (Sayfa yenilenmeden URL güncellenir)
  useEffect(() => {
    const params = new URLSearchParams();

    if (debouncedSearch) params.set('q', debouncedSearch);
    if (page > 1) params.set('page', page.toString());
    if (useTmdbSearch) params.set('tmdb', 'true');
    if (sortBy !== 'date') params.set('sortBy', sortBy);
    if (sortOrder !== 'desc') params.set('sortOrder', sortOrder);
    if (minYear) params.set('minYear', minYear);
    if (maxYear) params.set('maxYear', maxYear);
    if (minRating) params.set('minRating', minRating);
    if (maxRating) params.set('maxRating', maxRating);

    // URL'i arka planda güncelle (sayfa scroll'unu sıfırlamadan)
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
  }, [debouncedSearch, page, useTmdbSearch, sortBy, sortOrder, minYear, maxYear, minRating, maxRating, pathname, router]);

  // 3. Verileri Çek
  useEffect(() => {
    const fetchMovies = async () => {
      setLoading(true);
      try {
        if (useTmdbSearch) {
          if (!debouncedSearch.trim()) {
            setMovies([]);
            setTotalPages(1);
            setTotalRecords(0);
            return;
          }
          const res = await movieService.searchTmdb(debouncedSearch, page);
          setMovies(res.data ?? []);
          setTotalPages(res.totalPages || 1);
          setTotalRecords(res.totalRecords || 0);
        } else {
          const query: GetMoviesQuery = {
            searchTerm: debouncedSearch || undefined,
            sortBy,
            sortOrder,
            minYear: minYear ? Number(minYear) : undefined,
            maxYear: maxYear ? Number(maxYear) : undefined,
            minRating: minRating ? Number(minRating) : undefined,
            maxRating: maxRating ? Number(maxRating) : undefined,
            page,
            pageSize: 20,
          };
          const res = await movieService.getMovies(query);
          setMovies(res.data ?? []);
          setTotalPages(res.totalPages || 1);
          setTotalRecords(res.totalRecords || 0);
        }
      } catch (err) {
        console.error('Film listesi yüklenirken hata oluştu:', err);
        setMovies([]);
      } finally {
        setLoading(false);
      }
    };

    fetchMovies();
  }, [debouncedSearch, sortBy, sortOrder, minYear, maxYear, minRating, maxRating, useTmdbSearch, page]);

  // Filme Tıklama
  const handleMovieClick = async (movie: Movie) => {

    sessionStorage.setItem('catalog_scroll_pos', window.scrollY.toString());
    if (useTmdbSearch && movie.tmdbId) {
      try {
        setSyncingTmdbId(movie.tmdbId);
        const syncedMovie = await movieService.syncMovie(movie.tmdbId);
        router.push(`/movies/${syncedMovie.id}`);
      } catch (err) {
        console.error('TMDB Filmi senkronize edilemedi:', err);
      } finally {
        setSyncingTmdbId(null);
      }
    } else {
      router.push(`/movies/${movie.id}`);
    }
  };

  useEffect(() => {
  if (!loading && movies.length > 0) {
    const savedScrollPos = sessionStorage.getItem('catalog_scroll_pos');
    
    if (savedScrollPos) {
      window.scrollTo({
        top: Number(savedScrollPos),
        behavior: 'instant', // Anında kaydır, yumuşak geçiş yapıp gözü yorma
      });
      // Tekrar tekrar kaydırmaması için kullandık sonra siliyoruz
      sessionStorage.removeItem('catalog_scroll_pos');
    }
  } 
}, [loading, movies]);

  return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-7xl mx-auto space-y-8">
        {/* Başlık ve Toplam Sayı */}
        <div>
          <h1 className="text-3xl font-extrabold text-slate-100">Film Kataloğu</h1>
          <p className="text-slate-400 text-sm mt-1">
            Toplam <span className="text-indigo-400 font-semibold">{totalRecords}</span> film listeleniyor.
          </p>
        </div>

        {/* Kontrol Paneli */}
        <div className="bg-slate-900 p-4 rounded-xl border border-slate-800 space-y-4">
          <div className="flex flex-col lg:flex-row gap-4 justify-between items-stretch lg:items-center">
            {/* Arama Input */}
            <div className="relative flex-1">
              <input
                type="text"
                placeholder={useTmdbSearch ? "TMDB'de canlı film ara..." : "Katalogda film ara..."}
                value={searchTerm}
                onChange={(e) => {
                  setSearchTerm(e.target.value);
                  setPage(1);
                }}
                className="w-full px-4 py-2.5 rounded-lg bg-slate-800 border border-slate-700 text-white placeholder-slate-500 focus:outline-none focus:border-indigo-500 text-sm"
              />
            </div>

            {/* Kontroller */}
            <div className="flex flex-wrap items-center gap-3">
              {!useTmdbSearch && (
                <>
                  <button
                    onClick={() => setShowFilters(!showFilters)}
                    className={`px-3 py-2 rounded-lg border text-xs font-medium transition-colors ${
                      showFilters ? 'bg-indigo-600 border-indigo-500 text-white' : 'bg-slate-800 border-slate-700 text-slate-300 hover:bg-slate-700'
                    }`}
                  >
                    Filtreler {showFilters ? '▲' : '▼'}
                  </button>

                  <select
                    value={sortBy}
                    onChange={(e) => {
                      setSortBy(e.target.value as any);
                      setPage(1);
                    }}
                    className="px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-xs text-slate-200 focus:outline-none"
                  >
                    <option value="date">Ekleme Tarihi</option>
                    <option value="vote">Puan (TMDB)</option>
                    <option value="votecount">Oy Sayısı</option>
                  </select>

                  <button
                    onClick={() => {
                      setSortOrder(sortOrder === 'asc' ? 'desc' : 'asc');
                      setPage(1);
                    }}
                    className="px-3 py-2 bg-slate-800 border border-slate-700 rounded-lg text-xs text-slate-300 hover:bg-slate-700"
                  >
                    {sortOrder === 'asc' ? '↑ Artan' : '↓ Azalan'}
                  </button>
                </>
              )}

              <label className="flex items-center gap-2 cursor-pointer bg-slate-800/60 px-3 py-2 rounded-lg border border-slate-700/80 text-xs text-slate-300 hover:bg-slate-800">
                <input
                  type="checkbox"
                  checked={useTmdbSearch}
                  onChange={(e) => {
                    setUseTmdbSearch(e.target.checked);
                    setSearchTerm('');
                    setPage(1);
                  }}
                  className="rounded bg-slate-900 border-slate-700 text-indigo-600 focus:ring-0"
                />
                TMDB Global Araması
              </label>
            </div>
          </div>

          {/* Akordeon Filtre Paneli */}
          {showFilters && !useTmdbSearch && (
            <div className="pt-4 border-t border-slate-800 grid grid-cols-2 sm:grid-cols-4 gap-4">
              <div>
                <label className="block text-[11px] text-slate-400 mb-1">Min Yıl</label>
                <input
                  type="number"
                  placeholder="1990"
                  value={minYear}
                  onChange={(e) => { setMinYear(e.target.value); setPage(1); }}
                  className="w-full px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-xs text-white"
                />
              </div>
              <div>
                <label className="block text-[11px] text-slate-400 mb-1">Max Yıl</label>
                <input
                  type="number"
                  placeholder="2024"
                  value={maxYear}
                  onChange={(e) => { setMaxYear(e.target.value); setPage(1); }}
                  className="w-full px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-xs text-white"
                />
              </div>
              <div>
                <label className="block text-[11px] text-slate-400 mb-1">Min Puan</label>
                <input
                  type="number"
                  step="0.1"
                  placeholder="7.0"
                  value={minRating}
                  onChange={(e) => { setMinRating(e.target.value); setPage(1); }}
                  className="w-full px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-xs text-white"
                />
              </div>
              <div>
                <label className="block text-[11px] text-slate-400 mb-1">Max Puan</label>
                <input
                  type="number"
                  step="0.1"
                  placeholder="10"
                  value={maxRating}
                  onChange={(e) => { setMaxRating(e.target.value); setPage(1); }}
                  className="w-full px-3 py-1.5 bg-slate-800 border border-slate-700 rounded text-xs text-white"
                />
              </div>
            </div>
          )}
        </div>

        {/* Film Izgarası */}
        {loading ? (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {[...Array(10)].map((_, i) => (
              <div key={i} className="h-80 bg-slate-900 rounded-xl animate-pulse border border-slate-800" />
            ))}
          </div>
        ) : movies.length === 0 ? (
          <div className="text-center py-20 bg-slate-900/50 rounded-xl border border-slate-800/60">
            <p className="text-slate-400">Aramanıza veya filtrelerinize uygun film bulunamadı.</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {movies.map((movie) => {
              const isSyncing = syncingTmdbId === movie.tmdbId;

              return (
                <div
                  key={movie.id || movie.tmdbId}
                  onClick={() => handleMovieClick(movie)}
                  className="group relative bg-slate-900 rounded-xl overflow-hidden border border-slate-800 hover:border-slate-700 cursor-pointer transition-all duration-300 flex flex-col"
                >
                  <div className="aspect-[2/3] w-full bg-slate-950 relative overflow-hidden">
                    {movie.posterUrl ? (
                      <img
                        src={movie.posterUrl}
                        alt={movie.title}
                        className="w-full h-full object-cover group-hover:scale-105 transition duration-300"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center text-xs text-slate-600">
                        Görsel Yok
                      </div>
                    )}

                    {isSyncing && (
                      <div className="absolute inset-0 bg-slate-950/80 flex flex-col items-center justify-center p-2 text-center">
                        <span className="text-xs text-indigo-400 animate-pulse">İçeri aktarılıyor...</span>
                      </div>
                    )}
                  </div>

                  <div className="p-4 flex-1 flex flex-col justify-between space-y-2">
                    <div>
                      <h3 className="font-bold text-sm text-slate-100 truncate group-hover:text-indigo-400 transition-colors">
                        {movie.title}
                      </h3>
                      {movie.genres && movie.genres.length > 0 && (
                        <p className="text-[11px] text-slate-500 truncate mt-0.5">
                          {movie.genres.join(', ')}
                        </p>
                      )}
                    </div>

                    <div className="flex justify-between items-center text-xs pt-2 border-t border-slate-800/80">
                      <span className="text-yellow-400 font-semibold">
                        ★ {(movie.rating ?? 0).toFixed(1)}
                      </span>
                      {movie.localVoteCount > 0 && (
                        <span className="text-[10px] text-indigo-400">
                          Platform: {movie.localVoteAverage} ({movie.localVoteCount})
                        </span>
                      )}
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {/* Sayfalama Kontrolleri */}
        {totalPages > 1 && (
          <div className="flex justify-center items-center gap-4 pt-6 border-t border-slate-800">
            <button
              onClick={() => setPage((prev) => Math.max(prev - 1, 1))}
              disabled={page === 1 || loading}
              className="px-4 py-2 bg-slate-900 border border-slate-800 rounded-lg text-sm font-medium text-slate-300 hover:bg-slate-800 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer transition-colors"
            >
              ← Önceki
            </button>

            <span className="text-xs text-slate-400">
              Sayfa <strong className="text-white">{page}</strong> / {totalPages}
            </span>

            <button
              onClick={() => setPage((prev) => Math.min(prev + 1, totalPages))}
              disabled={page === totalPages || loading}
              className="px-4 py-2 bg-slate-900 border border-slate-800 rounded-lg text-sm font-medium text-slate-300 hover:bg-slate-800 disabled:opacity-40 disabled:cursor-not-allowed cursor-pointer transition-colors"
            >
              Sonraki →
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

// Next.js useSearchParams Suspense Sarmalayıcısı
export default function MovieCatalogPage() {
  return (
    <Suspense fallback={<div className="min-h-screen bg-slate-950 text-white p-12">Yükleniyor...</div>}>
      <CatalogContent />
    </Suspense>
  );
}