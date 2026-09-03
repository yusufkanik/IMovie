
export interface Movie {
  id: number;
  tmdbId: number;
  title: string;
  overview: string;
  posterUrl?: string;
  rating: number;
  voteCount: number;
  localVoteAverage: number;
  localVoteCount: number;
  genres: string[];
  releaseDate?: string;
}

export interface PagedResponse<T> {
  data: T[];
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  totalRecords: number;
}

export interface Person {
  personId: number;
  name: string;
  profilePath?: string;
}

export interface Cast extends Person {
  character: string;
}

export interface MovieDetails extends Movie {
  runtime: number;
  budget: number;
  revenue: number;
  trailerUrl?: string;
  directors: Person[];
  cast: Cast[];
  releaseDate?: string;
}
