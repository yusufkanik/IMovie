import {api} from '@/lib/api'
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
  userEmail: string;
  movies: any[]; // MovieResponseDto
  createdAt: string;
  movieCount: number;
}

export interface CreateCustomListDto {
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

  getListById: (id: number) => 
    api.get<CustomListDetail>(`lists/${id}`),

  createList: (dto: CreateCustomListDto) => 
    api.post<number>(`lists`, dto),

  deleteList: (id: number) => 
    api.delete(`lists/${id}`),

  addMovieToList: (listId: number, dto: AddMovieToListDto) => 
    api.post(`lists/${listId}/movies`, dto),

  removeMovieFromList: (listId: number, movieId: number) => 
    api.delete(`lists/${listId}/movies/${movieId}`)
};