import { api } from '@/lib/api';
import { Movie } from '@/types/movie';

export const favoriteService = {
  // Kullanıcının favori filmlerini getirir
  getUserFavorites: async (): Promise<Movie[]> => {
    const response = await api.get<Movie[]>('/favorites');
    return response.data;
  },

  // Filme favori ekler
  addFavorite: async (movieId: number): Promise<void> => {
    await api.post(`/favorites/${movieId}`);
  },

  // Film favorilerden çıkarır
  removeFavorite: async (movieId: number): Promise<void> => {
    await api.delete(`/favorites/${movieId}`);
  },
};