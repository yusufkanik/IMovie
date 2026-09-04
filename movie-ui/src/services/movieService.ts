import { api } from '@/lib/api';
import { Movie, MovieDetails, PagedResponse } from '@/types/movie';
import { CreateReviewDto, ReviewResponseDto } from '@/components/movieReview';

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

  getPersonalizedRecommendations: async (): Promise<Movie[]> => {
    const response = await api.get<Movie[]>(`/movies/recommendations`);
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

  getMovieReviews: async (movieId: number): Promise<ReviewResponseDto[]> => {
    const res = await api.get(`/movies/${movieId}/reviews`);
    return res.data;
  },

  // Yeni Yorum/Puan Ekle
  addReview: async (movieId: number, dto: CreateReviewDto): Promise<ReviewResponseDto> => {
    const res = await api.post(`/movies/${movieId}/reviews`, dto);
    return res.data;
  },

  updateReview: async (movieId: number, reviewId: number, dto: CreateReviewDto): Promise<ReviewResponseDto> => {
    const res = await api.put(`/movies/${movieId}/reviews`, dto);
    return res.data;
  },

  // Yorum Sil
  deleteReview: async (movieId: number, reviewId: number): Promise<void> => {
    await api.delete(`/movies/${movieId}/reviews`);
  },

  // Beğen / Beğenme
  likeReview: async (movieId: number, reviewId: number): Promise<void> => {
    await api.post(`/movies/${movieId}/reviews/${reviewId}/like`);
  },

  dislikeReview: async (movieId: number, reviewId: number): Promise<void> => {
    await api.post(`/movies/${movieId}/reviews/${reviewId}/dislike`);
  },

};