import { api } from '@/lib/api';
import { Movie, MovieDetails, PagedResponse } from '@/types/movie';

export interface GetMoviesQuery {
  searchTerm?: string;
  sortBy?: 'vote' | 'votecount' | 'date';
  sortOrder?: 'asc' | 'desc';
  minYear?: number;
  maxYear?: number;
  minRating?: number;
  maxRating?: number;
  genreIds?: number[];
  page?: number;
  pageSize?: number;
}

export const movieService = {
  // Yerel DB'den filtreli film listesi
  getMovies: async (query: GetMoviesQuery = {}): Promise<PagedResponse<Movie>> => {
    const response = await api.get<PagedResponse<Movie>>('/movies', { params: query });
    return response.data;
  },

  // ID'ye göre tekil film detayını getirme
  getMovieById: async (id: number): Promise<MovieDetails> => {
    const response = await api.get<MovieDetails>(`/movies/${id}`);
    return response.data;
  },

  // Benzer filmleri getirme
  getSimilarMovies: async (movieId: number): Promise<Movie[]> => {
    const response = await api.get<Movie[]>(`/movies/${movieId}/similar`);
    return response.data;
  },

  // TMDB üzerinde canlı arama
  searchTmdb: async (query: string, page = 1): Promise<PagedResponse<Movie>> => {
    const response = await api.get<PagedResponse<Movie>>('/movies/tmdb/search', {
      params: { query, page },
    });
    return response.data;
  },

  // TMDB filmini yerel DB'ye kaydetme ve detayını alma
  syncMovie: async (tmdbId: number): Promise<MovieDetails> => {
    const response = await api.post<MovieDetails>(`/movies/sync/${tmdbId}`);
    return response.data;
  },
};