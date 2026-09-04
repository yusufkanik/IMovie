'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/context/AuthContext';
import { Movie } from '@/types/movie';
import { movieService } from '@/services/movieService';

export default function HomePage() {
  const router = useRouter();
  const { user, isAuthenticated, loading, logout } = useAuth();

  const [movies, setMovies] = useState<Movie[]>([]);
  const [fetchingMovies, setFetchingMovies] = useState(true);
  const [pageInfo, setPageInfo] = useState({ pageNumber: 1, totalPages: 1, totalRecords: 0 });

  const [recommendations, setRecommendations] = useState<Movie[]>([]);
  const [recLoading, setRecLoading] = useState(false);

  useEffect(() => {
    if (loading) return;

    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    // personalized recommendations
    setRecLoading(true);
    movieService
      .getPersonalizedRecommendations()
      .then((data) => setRecommendations(data || []))
      .catch((err) => console.error('Öneriler yüklenemedi', err))
      .finally(() => setRecLoading(false));

    // popular movies
    setFetchingMovies(true);
    movieService
      .getMovies({ page: 1, pageSize: 12 })
      .then((res: any) => {
        setMovies(res.items || res.data || []);
        setPageInfo({
          pageNumber: res.pageNumber || res.page || 1,
          totalPages: res.totalPages || 1,
          totalRecords: res.totalRecords || 0,
        });
      })
      .catch((err) => console.error('Filmler yüklenemedi', err))
      .finally(() => setFetchingMovies(false));

  }, [isAuthenticated, loading, router]);

  if (loading) {
    return (
      <div className="min-h-screen bg-slate-950 text-white flex items-center justify-center">
        <p className="text-slate-400 animate-pulse">Oturum doğrulanıyor...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-8">
      {/* Header */}
      <header className="max-w-5xl mx-auto flex justify-between items-center pb-8 border-b border-slate-800">
        <div>
          <h1 className="text-2xl font-bold text-indigo-400">Film SaaS Platformu</h1>
          <p className="text-sm text-slate-400">
            Hoş geldin, <span className="text-slate-200 font-medium">{user?.username || user?.email}</span>!
          </p>
        </div>
        <button
          onClick={logout}
          className="px-4 py-2 bg-red-600/20 text-red-400 hover:bg-red-600/30 border border-red-500/30 rounded-lg text-sm transition-colors cursor-pointer"
        >
          Çıkış Yap
        </button>
      </header>

      {/* Main Content */}
      <main className="max-w-5xl mx-auto mt-8 space-y-10">

        {/* 1. SANA ÖZEL ÖNERİLER BÖLÜMÜ (Yatay Slider) */}
        <section>
          <div className="mb-4">
            <h2 className="text-xl font-semibold text-slate-200 flex items-center gap-2">
              <span>✨</span> Sana Özel Öneriler
            </h2>
            <p className="text-xs text-slate-400 mt-1">
              Favori türlerine ve izleme geçmişine göre seçtiğimiz filmler
            </p>
          </div>

          {recLoading ? (
            <div className="flex gap-4 overflow-x-auto pb-4 scrollbar-none">
              {[...Array(5)].map((_, i) => (
                <div
                  key={i}
                  className="w-40 h-60 bg-slate-900 border border-slate-800 rounded-xl animate-pulse shrink-0"
                />
              ))}
            </div>
          ) : recommendations.length > 0 ? (
            <div className="flex gap-4 overflow-x-auto pb-4 scrollbar-thin scrollbar-thumb-slate-800 scrollbar-track-transparent">
              {recommendations.map((movie) => (
                <div
                  key={movie.id}
                  onClick={() => router.push(`/movies/${movie.id}`)}
                  className="w-40 shrink-0 bg-slate-900 border border-slate-800 hover:border-indigo-500/50 rounded-xl overflow-hidden cursor-pointer transition-all duration-200 hover:-translate-y-1 hover:shadow-lg hover:shadow-indigo-500/10 group"
                >
                  <div className="relative h-56 bg-slate-800 overflow-hidden">
                    {movie.posterUrl ? (
                      <img
                        src={movie.posterUrl}
                        alt={movie.title}
                        className="w-full h-full object-cover group-hover:scale-105 transition-transform duration-300"
                      />
                    ) : (
                      <div className="w-full h-full flex items-center justify-center text-xs text-slate-500">
                        Görsel Yok
                      </div>
                    )}
                    <div className="absolute top-2 right-2 bg-slate-950/80 backdrop-blur-md px-2 py-0.5 rounded-md border border-slate-700/50 text-[10px] font-bold text-yellow-400">
                      ★ {(movie.rating ?? 0).toFixed(1)}
                    </div>
                  </div>

                  <div className="p-3">
                    <h3 className="font-semibold text-xs text-slate-200 truncate group-hover:text-indigo-400 transition-colors">
                      {movie.title}
                    </h3>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="p-4 bg-slate-900/40 border border-slate-800 rounded-xl text-xs text-slate-400 text-center">
              Sana özel öneriler oluşturabilmemiz için birkaç filmi favorilerine ekleyebilirsin!
            </div>
          )}
        </section>

        {/* 2. POPÜLER FİLMLER KATALOĞU */}
        <section>
          <div className="flex justify-between items-center mb-6">
            <h2 className="text-xl font-semibold text-slate-200">Popüler Filmler</h2>
            {pageInfo.totalRecords > 0 && (
              <span className="text-xs text-slate-400 bg-slate-900 border border-slate-800 px-3 py-1 rounded-full">
                Toplam {pageInfo.totalRecords} film
              </span>
            )}
          </div>

          {fetchingMovies ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
              {[...Array(6)].map((_, i) => (
                <div key={i} className="h-80 bg-slate-900 border border-slate-800 rounded-xl animate-pulse" />
              ))}
            </div>
          ) : movies.length > 0 ? (
            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
              {movies.map((movie) => (
                <div
                  key={movie.id}
                  onClick={() => router.push(`/movies/${movie.id}`)}
                  className="bg-slate-900 border border-slate-800 rounded-xl overflow-hidden shadow-lg flex flex-col hover:border-slate-700 transition-all cursor-pointer group"
                >
                  {movie.posterUrl ? (
                    <img
                      src={movie.posterUrl}
                      alt={movie.title}
                      className="w-full h-64 object-cover group-hover:scale-102 transition-transform duration-200"
                    />
                  ) : (
                    <div className="w-full h-64 bg-slate-800 flex items-center justify-center text-slate-500 text-sm">
                      Görsel Yok
                    </div>
                  )}
                  <div className="p-4 flex-1 flex flex-col justify-between">
                    <div>
                      <h3 className="font-bold text-lg text-slate-100 group-hover:text-indigo-400 transition-colors">
                        {movie.title}
                      </h3>
                      <p className="text-slate-400 text-xs mt-1 line-clamp-2">{movie.overview}</p>
                    </div>
                    <div className="mt-4 flex items-center justify-between">
                      <div className="flex gap-1 flex-wrap">
                        {movie.genres?.map((genre, index) => (
                          <span key={index} className="text-[10px] px-2 py-0.5 bg-indigo-500/10 text-indigo-400 rounded-full">
                            {genre}
                          </span>
                        ))}
                      </div>
                      <span className="text-xs font-semibold text-yellow-400">
                        ★ {(movie.rating ?? 0).toFixed(1)}
                      </span>
                    </div>
                  </div>
                </div>
              ))}
            </div>
          ) : (
            <div className="p-6 bg-slate-900/50 border border-slate-800 rounded-xl text-center text-slate-400">
              Henüz gösterilecek film bulunamadı.
            </div>
          )}
        </section>

      </main>
    </div>
  );
}