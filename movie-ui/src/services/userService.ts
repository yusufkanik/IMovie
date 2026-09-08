import {api} from '@/lib/api'; // veya kullandığınız axios / fetch instance'ı
import { Movie } from '@/types/movie';

export enum WatchStatus {
  Watched = 2,
  PlanToWatch = 1,
  Dropped = 3,
}

export interface UserStats {
  totalWatchedMovies: number;
  totalPlanToWatch: number;
  totalDropped: number;
  averageRatingGiven: number;
}

export const userService = {

    getWatchStatus: async (movieId: number): Promise<WatchStatus | null> => {
    try {
      const response = await api.get(`/Users/me/movies/${movieId}/status`);
      return response.data.status ?? null;
    } catch (error) {
      console.error('İzleme durumu çekilemedi:', error);
      return null;
    }
  },

  // Kullanıcı istatistiklerini getir
  getStats: async (): Promise<UserStats> => {
    const res = await api.get<UserStats>('/Users/me/stats');
    return res.data;
  },

  // İzleme durumuna göre filmleri getir
  getMoviesByStatus: async (status: WatchStatus): Promise<Movie[]> => {
    const res = await api.get<Movie[]>(`/Users/me/movies/${status}`);
    return res.data;
  },

  setWatchStatus: async (movieId: number, status: WatchStatus): Promise<void> => {
    await api.put(`/Users/me/movies/${movieId}`, { status });
  },
};