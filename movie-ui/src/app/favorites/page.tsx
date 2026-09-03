'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { favoriteService } from '@/services/favoriteService';
import { Movie } from '@/types/movie';
import { useAuth } from '@/context/AuthContext';

export default function FavoritesPage() {
    const router = useRouter();
    const {isAuthenticated, loading: authLoading} = useAuth();
    const [favorites, setFavorites] = useState<Movie[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        favoriteService.getUserFavorites()
                .then(setFavorites)
                .catch(console.error)
                .finally(() => setLoading(false));

    }, [isAuthenticated, authLoading, router])

    if (loading || authLoading) {
    return <div className="min-h-screen bg-slate-950 text-white p-12">Yükleniyor...</div>;
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-7xl mx-auto space-y-8">
        <h1 className="text-3xl font-extrabold text-slate-100">Favori Filmlerim</h1>

        {favorites.length === 0 ? (
          <div className="text-center py-20 bg-slate-900/50 rounded-xl border border-slate-800">
            <p className="text-slate-400">Henüz favorilerinize film eklemediniz.</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {favorites.map((movie) => (
              <div
                key={movie.id}
                onClick={() => router.push(`/movies/${movie.id}`)}
                className="bg-slate-900 rounded-xl overflow-hidden border border-slate-800 cursor-pointer hover:border-slate-700 transition-all"
              >
                <div className="aspect-[2/3] w-full bg-slate-950">
                  {movie.posterUrl && (
                    <img src={movie.posterUrl} alt={movie.title} className="w-full h-full object-cover" />
                  )}
                </div>
                <div className="p-3">
                  <h3 className="font-bold text-xs truncate text-slate-200">{movie.title}</h3>
                  <span className="text-yellow-400 text-[10px] block mt-1">★ {(movie.rating ?? 0).toFixed(1)}</span>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </div>
  );
} 