'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { useAuth } from '@/context/AuthContext';
import { api } from '@/lib/api';
import { Movie, PagedResponse } from '@/types/movie';

export default function HomePage() {
  const router = useRouter();
  const { user, isAuthenticated, loading, logout } = useAuth();
  
  const [movies, setMovies] = useState<Movie[]>([]);
  const [fetchingMovies, setFetchingMovies] = useState(true);
  const [pageInfo, setPageInfo] = useState({ pageNumber: 1, totalPages: 1, totalRecords: 0 });

  useEffect(() => {
    if (loading) return;

    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    const fetchMovies = async () => {
      try {
        const response = await api.get<PagedResponse<Movie>>('/movies');
        
        setMovies(response.data.data);
        setPageInfo({
          pageNumber: response.data.pageNumber,
          totalPages: response.data.totalPages,
          totalRecords: response.data.totalRecords,
        });
      } catch (error) {
        console.error('Filmler çekilirken hata oluştu:', error);
      } finally {
        setFetchingMovies(false);
      }
    };

    fetchMovies();
  }, [loading, isAuthenticated, router]);

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
      <main className="max-w-5xl mx-auto mt-8">
        <div className="flex justify-between items-center mb-6">
          <h2 className="text-xl font-semibold text-slate-200">Popüler Filmler</h2>
          {pageInfo.totalRecords > 0 && (
            <span className="text-xs text-slate-400 bg-slate-900 border border-slate-800 px-3 py-1 rounded-full">
              Toplam {pageInfo.totalRecords} film
            </span>
          )}
        </div>

        {fetchingMovies ? (
          <p className="text-slate-500">Filmler yükleniyor...</p>
        ) : movies.length > 0 ? (
          <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6">
            {movies.map((movie) => (
              <div
                key={movie.id}
                className="bg-slate-900 border border-slate-800 rounded-xl overflow-hidden shadow-lg flex flex-col hover:border-slate-700 transition-all"
              >
                {movie.posterUrl ? (
                  <img
                    src={movie.posterUrl}
                    alt={movie.title}
                    className="w-full h-64 object-cover"
                  />
                ) : (
                  <div className="w-full h-64 bg-slate-800 flex items-center justify-center text-slate-500 text-sm">
                    Görsel Yok
                  </div>
                )}
                <div className="p-4 flex-1 flex flex-col justify-between">
                  <div>
                    <h3 className="font-bold text-lg text-slate-100">{movie.title}</h3>
                    <p className="text-slate-400 text-xs mt-1 line-clamp-2">{movie.overview}</p>
                  </div>
                  <div className="mt-4 flex items-center justify-between">
                    <div className="flex gap-1 flex-wrap">
                      {movie.genres.map((genre, index) => (
                        <span key={index} className="text-[10px] px-2 py-0.5 bg-indigo-500/10 text-indigo-400 rounded-full">
                          {genre}
                        </span>
                      ))}
                    </div>
                    <span className="text-xs font-semibold text-yellow-400">
                      ★ {movie.rating.toFixed(1)}
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
      </main>
    </div>
  );
}