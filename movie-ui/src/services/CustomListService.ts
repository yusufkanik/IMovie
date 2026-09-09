import {api} from '@/lib/api'
import { Movie } from '@/types/movie';

export interface CustomListSummary {
  id: number;
  title: string;
  description: string;
  isPublic: boolean;
  movieCount: number;
  createdAt: string;
}

export interface CustomListDetail {
  id: number;
  title: string;
  description: string;
  isPublic: boolean;
  ownerEmail: string;
  movies: Movie[]; // MovieResponseDto
  createdAt: string;
  movieCount: number;
}

export interface CreateCustomListDto {
  title: string;
  description: string;
  isPublic: boolean;
}

export interface UpdateCustomListDto {
  title: string;
  description: string;
  isPublic: boolean;
}

export interface AddMovieToListDto {
    movieId: number;
}

export const customListService = {
  getUserLists: () => 
    api.get<CustomListSummary[]>(`lists/me`),

  getPublicLists: () => api.get<CustomListSummary[]>(`lists/public`),

  getListById: (id: number) => 
    api.get<CustomListDetail>(`lists/${id}`),

  createList: (dto: CreateCustomListDto) => 
    api.post<number>(`lists`, dto),

  updateList: (id: number, dto: UpdateCustomListDto) => api.put(`lists/${id}`, dto),

  deleteList: (id: number) => 
    api.delete(`lists/${id}`),

  addMovieToList: (listId: number, dto: AddMovieToListDto) => 
    api.post(`lists/${listId}/movies`, dto),

  removeMovieFromList: (listId: number, movieId: number) => 
    api.delete(`lists/${listId}/movies/${movieId}`)
};