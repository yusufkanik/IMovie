'use client';

import { useEffect, useState } from 'react';
import { useRouter } from 'next/navigation';
import { userService, WatchStatus, UserStats } from '@/services/userService';
import { Movie } from '@/types/movie';
import { useAuth } from '@/context/AuthContext'; // 1. AuthContext'i içe aktarın

export default function ProfilePage() {
  const router = useRouter();
  const { isAuthenticated, loading: isAuthLoading } = useAuth(); // 2. Auth durumlarını alın

  const [stats, setStats] = useState<UserStats | null>(null);
  const [activeStatus, setActiveStatus] = useState<WatchStatus>(WatchStatus.Watched);
  const [movies, setMovies] = useState<Movie[]>([]);
  
  const [loadingStats, setLoadingStats] = useState(true);
  const [loadingMovies, setLoadingMovies] = useState(true);

  // Giriş yapılmamışsa Login'e yönlendir
  useEffect(() => {
    if (!isAuthLoading && !isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, isAuthLoading, router]);

  // 1. İstatistikleri Çek
  useEffect(() => {
    // Auth yükleniyorsa veya kullanıcı giriş yapmamışsa isteği engelle
    if (isAuthLoading || !isAuthenticated) return;

    const fetchStats = async () => {
      try {
        const data = await userService.getStats();
        setStats(data);
      } catch (err) {
        console.error('İstatistikler yüklenirken hata oluştu:', err);
      } finally {
        setLoadingStats(false);
      }
    };

    fetchStats();
  }, [isAuthenticated, isAuthLoading]);

  // 2. Filmleri Çek
  useEffect(() => {
    // Auth yükleniyorsa veya kullanıcı giriş yapmamışsa isteği engelle
    if (isAuthLoading || !isAuthenticated) return;

    const fetchMovies = async () => {
      setLoadingMovies(true);
      try {
        const data = await userService.getMoviesByStatus(activeStatus);
        setMovies(data);
      } catch (err) {
        console.error('Filmler yüklenirken hata oluştu:', err);
        setMovies([]);
      } finally {
        setLoadingMovies(false);
      }
    };

    fetchMovies();
  }, [activeStatus, isAuthenticated, isAuthLoading]);

  if (isAuthLoading) {
    return <div className="min-h-screen bg-slate-950 text-white p-12">Yükleniyor...</div>;
  }


    const tabs = [
        { label: 'İzlenenler', status: WatchStatus.Watched, count: stats?.totalWatchedMovies},
        { label: 'İzlenecekler', status: WatchStatus.PlanToWatch, count: stats?.totalPlanToWatch },
        { label: 'Bırakılanlar', status: WatchStatus.Dropped, count: stats?.totalDropped },
    ];

    return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-7xl mx-auto space-y-10">
        
        {/* Sayfa Başlığı */}
        <div>
          <h1 className="text-3xl font-extrabold text-slate-100">Profilim & Kütüphanem</h1>
          <p className="text-slate-400 text-sm mt-1">İzleme alışkanlıklarınız ve kaydettiğiniz filmler.</p>
        </div>

        {/* İSTATİSTİK KARTLARI */}
        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
          {/* İzlenenler */}
          <div className="bg-slate-900 border border-slate-800 p-5 rounded-2xl space-y-1">
            <p className="text-xs text-slate-400 font-medium">Toplam İzlenen</p>
            {loadingStats ? (
              <div className="h-8 w-16 bg-slate-800 rounded animate-pulse" />
            ) : (
              <p className="text-2xl font-black text-indigo-400">{stats?.totalWatchedMovies ?? 0}</p>
            )}
          </div>

          {/* İzlenecekler */}
          <div className="bg-slate-900 border border-slate-800 p-5 rounded-2xl space-y-1">
            <p className="text-xs text-slate-400 font-medium">İzlenecekler</p>
            {loadingStats ? (
              <div className="h-8 w-16 bg-slate-800 rounded animate-pulse" />
            ) : (
              <p className="text-2xl font-black text-amber-400">{stats?.totalPlanToWatch ?? 0}</p>
            )}
          </div>

          {/* Bırakılanlar */}
          <div className="bg-slate-900 border border-slate-800 p-5 rounded-2xl space-y-1">
            <p className="text-xs text-slate-400 font-medium">Yarıda Bırakılan</p>
            {loadingStats ? (
              <div className="h-8 w-16 bg-slate-800 rounded animate-pulse" />
            ) : (
              <p className="text-2xl font-black text-rose-400">{stats?.totalDropped ?? 0}</p>
            )}
          </div>

          {/* Ortalama Puan */}
          <div className="bg-slate-900 border border-slate-800 p-5 rounded-2xl space-y-1">
            <p className="text-xs text-slate-400 font-medium">Ortalama Puanınız</p>
            {loadingStats ? (
              <div className="h-8 w-16 bg-slate-800 rounded animate-pulse" />
            ) : (
              <p className="text-2xl font-black text-yellow-400 flex items-center gap-1">
                ★ {stats?.averageRatingGiven?.toFixed(1) ?? '0.0'}
              </p>
            )}
          </div>
        </div>

        {/* SEKMELER (TABS) */}
        <div className="flex border-b border-slate-800 gap-2">
          {tabs.map((tab) => {
            const isActive = activeStatus === tab.status;
            return (
              <button
                key={tab.status}
                onClick={() => setActiveStatus(tab.status)}
                className={`pb-3 px-4 text-sm font-semibold transition-all cursor-pointer border-b-2 ${
                  isActive
                    ? 'border-indigo-500 text-indigo-400'
                    : 'border-transparent text-slate-400 hover:text-slate-200'
                }`}
              >
                {tab.label}
                {tab.count !== undefined && (
                  <span className={`ml-2 text-xs px-2 py-0.5 rounded-full ${
                    isActive ? 'bg-indigo-500/20 text-indigo-300' : 'bg-slate-800 text-slate-500'
                  }`}>
                    {tab.count}
                  </span>
                )}
              </button>
            );
          })}
        </div>

        {/* FILM IZGARASI */}
        {loadingMovies ? (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {[...Array(5)].map((_, i) => (
              <div key={i} className="h-80 bg-slate-900 rounded-xl animate-pulse border border-slate-800" />
            ))}
          </div>
        ) : movies.length === 0 ? (
          <div className="text-center py-20 bg-slate-900/40 rounded-2xl border border-slate-800/60">
            <p className="text-slate-400 text-sm">Bu listede henüz hiç film bulunmuyor.</p>
          </div>
        ) : (
          <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
            {movies.map((movie) => (
              <div
                key={movie.id}
                onClick={() => router.push(`/movies/${movie.id}`)}
                className="group bg-slate-900 rounded-xl overflow-hidden border border-slate-800 hover:border-slate-700 cursor-pointer transition-all duration-300 flex flex-col justify-between"
              >
                {/* Poster */}
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
                </div>

                {/* İçerik */}
                <div className="p-4 flex-1 flex flex-col justify-between space-y-2">
                  <div>
                    <div className="flex items-start justify-between gap-2">
                      <h3 className="font-bold text-sm text-slate-100 truncate group-hover:text-indigo-400 transition-colors">
                        {movie.title}
                      </h3>
                      {movie.releaseDate && (
                        <span className="text-[11px] font-medium text-slate-400 shrink-0">
                          {new Date(movie.releaseDate).getFullYear()}
                        </span>
                      )}
                    </div>

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
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

      </div>
    </div>
  );
}