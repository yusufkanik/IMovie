'use client';
import React, { useEffect, useState } from 'react';
import { customListService, CustomListSummary } from '../services/CustomListService';
import { useAuth } from '@/context/AuthContext';

interface AddToListModalProps {
  movieId: number;
  onClose: () => void;
}

export function AddToListModal({movieId, onClose}: AddToListModalProps) {

    const { user, loading: authLoading } = useAuth();
    const [lists, setLists] = useState<CustomListSummary[]>([]);
    const [loading, setLoading] = useState(true);
    const [addingListId, setAddingListId] = useState<number | null>(null);

    useEffect(() => {
    if (!authLoading && user) {
      customListService
        .getUserLists()
        .then((res) => setLists(res.data))
        .catch((err) => console.error('Listeler çekilemedi:', err))
        .finally(() => setLoading(false));
    } else if (!authLoading) {
      setLoading(false);
    }
    }, [authLoading, user]);
    
    const handleAdd =  async (listId: number) => {

        setAddingListId(listId);
        setLoading(true);

        try {
            await customListService.addMovieToList(listId, {movieId});
            alert("Film listeye başarıyla eklendi.");
            onClose();
        }
        catch(err: any) {
            alert(err.response?.data?.message || 'Film zaten bu listede mevcut veya eklenemedi.');
        }
        finally {
            setAddingListId(null);
            setLoading(false);
        }

    }

    return (
    <div className="fixed inset-0 bg-slate-950/80 backdrop-blur-sm flex items-center justify-center p-4 z-50">
      <div className="bg-slate-900 border border-slate-800 p-6 rounded-2xl max-w-md w-full shadow-2xl space-y-4">
        
        <div className="flex items-center justify-between border-b border-slate-800 pb-3">
          <h3 className="text-sm font-bold text-slate-100">Listeye Ekle</h3>
          <button 
            onClick={onClose}
            className="text-slate-400 hover:text-white text-xs font-semibold transition-colors"
          >
            ✕
          </button>
        </div>

        <div className="space-y-2 max-h-60 overflow-y-auto pr-1">
          {loading || authLoading ? (
            [...Array(3)].map((_, i) => (
              <div key={i} className="h-12 bg-slate-800/60 rounded-xl animate-pulse" />
            ))
          ) : !user ? (
            <p className="text-xs text-slate-400 py-4 text-center">
              Film eklemek için lütfen giriş yapın.
            </p>
          ) : lists.length === 0 ? (
            <p className="text-xs text-slate-400 py-4 text-center">
              Henüz hiç özel listeniz bulunmuyor.
            </p>
          ) : (
            lists.map((list) => {
              const isAdding = addingListId === list.id;
              return (
                <button
                  key={list.id}
                  disabled={addingListId !== null}
                  onClick={() => handleAdd(list.id)}
                  className="w-full p-3 bg-slate-800/50 hover:bg-slate-800 border border-slate-700/60 rounded-xl text-left flex justify-between items-center transition-colors disabled:opacity-50 cursor-pointer"
                >
                  <span className="text-xs font-semibold text-slate-200">{list.title}</span>
                  <span className="text-[11px] text-indigo-400 font-medium">
                    {isAdding ? 'Ekleniyor...' : `${list.movieCount} film`}
                  </span>
                </button>
              );
            })
          )}
        </div>

        <button
          onClick={onClose}
          disabled={addingListId !== null}
          className="w-full py-2.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-xl text-xs font-medium transition-colors cursor-pointer disabled:opacity-50"
        >
          Kapat
        </button>

      </div>
    </div>
  );

}