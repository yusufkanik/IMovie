'use client'
import React, { useEffect, useState } from 'react';
import { customListService, CustomListSummary } from '../services/CustomListService';
import { useAuth } from '@/context/AuthContext';

export default function UserLists() {
  const { user, loading: authLoading } = useAuth();

  const [lists, setLists] = useState<CustomListSummary[]>([]);
  const [loading, setLoading] = useState(true);
  const [submitting, setSubmitting] = useState(false);

  const [title, setTitle] = useState('');
  const [description, setDescription] = useState('');
  const [isPublic, setIsPublic] = useState(true);

  const fetchLists = async () => {
    try {
      const res = await customListService.getUserLists();
      setLists(res.data);
    } catch (err) {
      console.error('Listeler çekilemedi:', err);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    if (!authLoading) {
      if (user) {
        fetchLists();
      } else {
        setLoading(false);
      }
    }
  }, [authLoading, user]);

  const handleCreate = async (e: React.FormEvent) => {
    e.preventDefault();
    setSubmitting(true);
    try {
      await customListService.createList({ title, description, isPublic });
      setTitle('');
      setDescription('');
      await fetchLists();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Bu işleme yetkiniz yok.');
    } finally {
      setSubmitting(false);
    }
  };

  if (authLoading) {
    return (
      <div className="min-h-screen bg-slate-950 flex items-center justify-center text-slate-400 text-sm">
        Oturum doğrulanıyor...
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-5xl mx-auto space-y-8">
        <div>
          <h1 className="text-3xl font-extrabold text-slate-100">Özel Listelerim</h1>
          <p className="text-slate-400 text-sm mt-1">
            Kendi film koleksiyonlarınızı oluşturun ve yönetin.
          </p>
        </div>

        <form onSubmit={handleCreate} className="bg-slate-900 border border-slate-800 p-6 rounded-xl space-y-4">
          <h2 className="text-sm font-semibold text-slate-200">Yeni Liste Oluştur</h2>
          
          <input
            type="text"
            placeholder="Liste Başlığı"
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            required
            className="w-full bg-slate-800 border border-slate-700 rounded-lg px-4 py-2.5 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
          />

          <textarea
            placeholder="Açıklama (opsiyonel)"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            rows={2}
            className="w-full bg-slate-800 border border-slate-700 rounded-lg p-3 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500 resize-none"
          />

          <div className="flex items-center justify-between pt-2">
            <label className="flex items-center gap-2 text-xs text-slate-300 cursor-pointer">
              <input
                type="checkbox"
                checked={isPublic}
                onChange={(e) => setIsPublic(e.target.checked)}
                className="rounded bg-slate-900 border-slate-700 text-indigo-600 focus:ring-0 cursor-pointer"
              />
              Herkese Açık
            </label>

            <button
              type="submit"
              disabled={submitting}
              className="px-5 py-2.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-xs font-semibold transition-colors disabled:opacity-50 cursor-pointer"
            >
              {submitting ? 'Oluşturuluyor...' : 'Liste Oluştur'}
            </button>
          </div>
        </form>

        {loading ? (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {[...Array(4)].map((_, i) => (
              <div key={i} className="h-28 bg-slate-900 rounded-xl animate-pulse border border-slate-800" />
            ))}
          </div>
        ) : !user ? (
          <div className="text-center py-16 bg-slate-900/50 rounded-xl border border-slate-800/60">
            <p className="text-sm text-slate-400">Listelerinizi görmek için lütfen giriş yapın.</p>
          </div>
        ) : lists.length === 0 ? (
          <div className="text-center py-16 bg-slate-900/50 rounded-xl border border-slate-800/60">
            <p className="text-sm text-slate-400">Henüz hiç özel listeniz yok.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {lists.map((list) => (
              <div key={list.id} className="bg-slate-900 border border-slate-800 p-5 rounded-xl flex justify-between items-start hover:border-slate-700 transition-colors">
                <div>
                  <h3 className="font-bold text-sm text-slate-100">{list.title}</h3>
                  {list.description && (
                    <p className="text-xs text-slate-400 mt-1">{list.description}</p>
                  )}
                  <div className="mt-4 text-[11px] text-slate-500 flex items-center gap-3">
                    <span className="text-indigo-400 font-medium">{list.movieCount} Film</span>
                    <span>•</span>
                    <span>{list.isPublic ? '🌐 Herkese Açık' : '🔒 Özel'}</span>
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