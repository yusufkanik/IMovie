'use client';

import {useEffect, useState, use} from 'react'
import {useRouter} from 'next/navigation'
import {api} from '@/lib/api'
import { MovieDetails, Movie } from '@/types/movie';
import { useAuth } from '@/context/AuthContext';

export default function MovieDetailsPage({ params }: { params: Promise<{ id: string }> }) {
    const resolvedParams = use(params);
    const movieId = resolvedParams.id;

    const router = useRouter();
    const {isAuthenticated, loading: authLoading} = useAuth();

    const [movie, setMovie] = useState<MovieDetails | null>(null);
    const [similarMovies, setSimilarMovies] = useState<Movie[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState('');

    useEffect(() => {
        if (authLoading) return;
        if (!isAuthenticated) {
            router.push("/login");
            return;
        }

        const fetchMovieData = async () => {
            try {
                setLoading(true);
                const [movieRes, similarRes] = await Promise.all([
                    api.get<MovieDetails>(`/movies/${movieId}`),
                    api.get<Movie[]>(`/movies/${movieId}/similar`)
                ]);

                setMovie(movieRes.data);
                setSimilarMovies(similarRes.data);
            } 
            catch(err: any) {
                console.error(err);
                setError('Film detayları yüklenirken bir hata oluştu.');
            }
            finally {
                setLoading(false);
            }
        };

        fetchMovieData();
    }, [movieId, isAuthenticated, authLoading, router]);

    if (authLoading || loading) {

        return (
        <div className="min-h-screen bg-slate-950 text-white flex items-center justify-center">
            <p className="text-slate-400 animate-pulse">Film yükleniyor...</p>
        </div>
        );
    }

    if (error || !movie) {

        return (
        <div className="min-h-screen bg-slate-950 text-white p-8 flex flex-col items-center justify-center">
            <p className="text-red-400 mb-4">{error || 'Film bulunamadı.'}</p>
            <button
            onClick={() => router.push('/')}
            className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-white rounded-lg text-sm transition-colors"
            >
            Ana Sayfaya Dön
            </button>
        </div>
        );
    }

    // Youtube URL'sinden Embed ID çıkarma
    const getEmbedYoutubeUrl = (url?: string) => {
        if (!url) return null;
        const regExp = /^.*(youtu.be\/|v\/|u\/\w\/|embed\/|watch\?v=|\&v=)([^#\&\?]*).*/;
        const match = url.match(regExp);
        return (match && match[2].length === 11) ? `https://www.youtube.com/embed/${match[2]}` : null;
    };

    const embedTrailerUrl = getEmbedYoutubeUrl(movie.trailerUrl);

    return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-6xl mx-auto space-y-12">
        {/* Geri Dön Butonu */}
        <button
          onClick={() => router.back()}
          className="text-sm text-slate-400 hover:text-white transition-colors flex items-center gap-2 cursor-pointer"
        >
          ← Geri Dön
        </button>

        {/* Hero & Ana Bilgiler */}
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8 items-start">
          {/* Poster */}
          <div className="rounded-xl overflow-hidden bg-slate-900 border border-slate-800 shadow-2xl">
            {movie.posterUrl ? (
              <img src={movie.posterUrl} alt={movie.title} className="w-full h-auto object-cover" />
            ) : (
              <div className="h-96 flex items-center justify-center text-slate-600">Görsel Yok</div>
            )}
          </div>

          {/* Film Metadataları */}
          <div className="md:col-span-2 space-y-6">
            <div>
              <h1 className="text-3xl md:text-4xl font-extrabold text-slate-100">{movie.title}</h1>
              <div className="flex flex-wrap items-center gap-3 mt-3 text-sm text-slate-400">
                <span className="text-yellow-400 font-semibold">★ {movie.rating.toFixed(1)}</span>
                <span>•</span>
                <span>{movie.runtime} dk</span>
                <span>•</span>
                <div className="flex gap-1.5 flex-wrap">
                  {movie.genres.map((g, i) => (
                    <span key={i} className="px-2 py-0.5 bg-indigo-500/10 text-indigo-400 rounded-md text-xs">
                      {g}
                    </span>
                  ))}
                </div>
              </div>
            </div>

            {/* Özet */}
            <div>
              <h3 className="text-sm font-semibold text-slate-300 uppercase tracking-wider mb-2">Özet</h3>
              <p className="text-slate-400 leading-relaxed text-sm md:text-base">{movie.overview}</p>
            </div>

            {/* Finansal & Yapım Detayları */}
            <div className="grid grid-cols-2 sm:grid-cols-3 gap-4 pt-4 border-t border-slate-800 text-xs">
              <div>
                <span className="text-slate-500 block">Bütçe</span>
                <span className="text-slate-200 font-medium">
                  {movie.budget > 0 ? `$${movie.budget.toLocaleString()}` : '-'}
                </span>
              </div>
              <div>
                <span className="text-slate-500 block">Hasılat</span>
                <span className="text-slate-200 font-medium">
                  {movie.revenue > 0 ? `$${movie.revenue.toLocaleString()}` : '-'}
                </span>
              </div>
              <div>
                <span className="text-slate-500 block">Lokal Puan (Platform)</span>
                <span className="text-slate-200 font-medium">
                  {movie.localVoteCount > 0 ? `${movie.localVoteAverage} (${movie.localVoteCount} Oy)` : 'Henüz Oylanmadı'}
                </span>
              </div>
            </div>

            {/* Yönetmenler */}
            {movie.directors.length > 0 && (
              <div className="pt-2">
                <h4 className="text-xs text-slate-500 mb-1">Yönetmen</h4>
                <p className="text-sm text-slate-200">{movie.directors.map(d => d.name).join(', ')}</p>
              </div>
            )}
          </div>
        </div>

        {/* Fragman (Varsa) */}
        {embedTrailerUrl && (
          <section className="space-y-4">
            <h2 className="text-xl font-bold text-slate-200">Fragman</h2>
            <div className="aspect-video w-full rounded-xl overflow-hidden border border-slate-800 bg-slate-900">
              <iframe
                src={embedTrailerUrl}
                title={`${movie.title} Fragman`}
                className="w-full h-full"
                allowFullScreen
              />
            </div>
          </section>
        )}

        {/* Oyuncu Kadrosu */}
        {movie.cast.length > 0 && (
          <section className="space-y-4">
            <h2 className="text-xl font-bold text-slate-200">Oyuncular</h2>
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-4">
              {movie.cast.map((actor) => (
                <div key={actor.personId} className="bg-slate-900 border border-slate-800/80 p-3 rounded-xl flex items-center gap-3">
                  {actor.profilePath ? (
                    <img
                      src={`https://image.tmdb.org/t/p/w185${actor.profilePath}`}
                      alt={actor.name}
                      className="w-12 h-12 rounded-full object-cover shrink-0"
                    />
                  ) : (
                    <div className="w-12 h-12 rounded-full bg-slate-800 shrink-0 flex items-center justify-center text-xs text-slate-500">
                      ?
                    </div>
                  )}
                  <div className="overflow-hidden">
                    <p className="text-xs font-semibold text-slate-200 truncate">{actor.name}</p>
                    <p className="text-[10px] text-slate-400 truncate">{actor.character}</p>
                  </div>
                </div>
              ))}
            </div>
          </section>
        )}

        {/* Benzer Filmler */}
        {similarMovies.length > 0 && (
          <section className="space-y-4 pt-8 border-t border-slate-800">
            <h2 className="text-xl font-bold text-slate-200">Benzer Filmler</h2>
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-5 gap-4">
              {similarMovies.map((m) => (
                <div
                  key={m.id}
                  onClick={() => router.push(`/movies/${m.id}`)}
                  className="bg-slate-900 border border-slate-800 hover:border-slate-700 rounded-xl overflow-hidden cursor-pointer transition-all"
                >
                  {m.posterUrl && (
                    <img src={m.posterUrl} alt={m.title} className="w-full h-48 object-cover" />
                  )}
                  <div className="p-3">
                    <h3 className="font-semibold text-xs text-slate-200 truncate">{m.title}</h3>
                    <span className="text-[10px] text-yellow-400 mt-1 block">★ {m.rating.toFixed(1)}</span>
                  </div>
                </div>
              ))}
            </div>
          </section>
        )}
      </div>
    </div>
  );
}