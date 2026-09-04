'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { favoriteService } from '@/services/favoriteService';
import { Movie } from '@/types/movie';
import { useAuth } from '@/context/AuthContext';

export default function FavoritesPage() {
  const router = useRouter();
  const { isAuthenticated, loading: authLoading } = useAuth();
  const [favorites, setFavorites] = useState<Movie[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (authLoading) return;

    if (!isAuthenticated) {
      router.push('/login');
      return;
    }

    favoriteService
      .getUserFavorites()
      .then(setFavorites)
      .catch(console.error)
      .finally(() => setLoading(false));
  }, [isAuthenticated, authLoading, router]);

  // Tek tıkla favoriden çıkarma fonksiyonu
  const handleRemoveFavorite = async (e: React.MouseEvent, movieId: number) => {
    e.stopPropagation(); // Kartın tıklama olayının (sayfaya gitme) tetiklenmesini engeller

    try {
      await favoriteService.removeFavorite(movieId);
      // Ekranı anında güncelle (state'ten kaldır)
      setFavorites((prev) => prev.filter((m) => m.id !== movieId));
    } catch (err) {
      console.error('Favorilerden çıkarılamadı:', err);
    }
  };

  if (loading || authLoading) {
    return (
      <div className="min-h-screen bg-slate-950 text-white p-12 flex items-center justify-center">
        <p className="text-slate-400 animate-pulse">Favoriler yükleniyor...</p>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-7xl mx-auto space-y-8">
        <div>
          <h1 className="text-2xl md:text-3xl font-extrabold text-slate-100">Favori Filmlerim</h1>
          <p className="text-xs text-slate-400 mt-1">
            Kaydettiğin filmler burada listelenir.
          </p>
        </div>

        {favorites.length === 0 ? (
          <div className="text-center py-20 bg-slate-900/50 rounded-xl border border-slate-800">
            <p className="text-slate-400 text-sm">Henüz favorilerinize film eklemediniz.</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {favorites.map((movie) => (
              <div
                key={movie.id}
                onClick={() => router.push(`/movies/${movie.id}`)}
                className="group relative bg-slate-900 rounded-xl overflow-hidden border border-slate-800 cursor-pointer hover:border-indigo-500/50 transition-all hover:-translate-y-1"
              >
                {/* Afiş ve Hızlı Silme Butonu */}
                <div className="relative aspect-[2/3] w-full bg-slate-950 overflow-hidden">
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

                  {/* Hızlı Favoriden Çıkar Butonu */}
                  <button
                    onClick={(e) => handleRemoveFavorite(e, movie.id)}
                    title="Favorilerden Çıkar"
                    className="absolute top-2 right-2 p-2 bg-slate-950/80 hover:bg-rose-600 text-rose-400 hover:text-white rounded-full border border-slate-700/50 transition-colors backdrop-blur-md opacity-90 sm:opacity-0 group-hover:opacity-100"
                  >
                    <svg
                      xmlns="http://www.w3.org/2000/svg"
                      viewBox="0 0 24 24"
                      fill="currentColor"
                      className="w-4 h-4"
                    >
                      <path d="M11.645 20.91l-.007-.003-.022-.012a15.247 15.247 0 01-.383-.218 25.18 25.18 0 01-4.244-3.17C4.688 15.36 2.25 12.174 2.25 8.25 2.25 5.322 4.714 3 7.688 3A5.5 5.5 0 0112 5.052 5.5 5.5 0 0116.313 3c2.973 0 5.437 2.322 5.437 5.25 0 3.925-2.438 7.111-4.739 9.256a25.175 25.175 0 01-4.244 3.17 15.247 15.247 0 01-.383.219l-.022.012-.007.004-.003.001a.752.752 0 01-.704 0l-.003-.001z" />
                    </svg>
                  </button>
                </div>

                <div className="p-3">
                  <h3 className="font-bold text-xs truncate text-slate-200 group-hover:text-indigo-400 transition-colors">
                    {movie.title}
                  </h3>
                  <span className="text-yellow-400 text-[10px] block mt-1 font-semibold">
                    ★ {(movie.rating ?? 0).toFixed(1)}
                  </span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}