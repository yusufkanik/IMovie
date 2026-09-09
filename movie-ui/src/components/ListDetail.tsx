'use client';

import React, { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import { customListService, CustomListDetail } from '@/services/CustomListService';
import { useAuth } from '@/context/AuthContext';

export default function ListDetailPage() {

    const params = useParams();
    const router = useRouter();
    const { user } = useAuth();

    const listId = Number(params.id);
    const [list, setList] = useState<CustomListDetail | null>(null);
    const [loading, setLoading] = useState(true);

    const fetchDetail = async () => {

        try {
            setLoading(true);
            const res = await customListService.getListById(listId);
            setList(res.data);
        }
        catch(err) {
            console.error("Liste çekilemedi", err);
        }
        finally {
            setLoading(false);
        }
    }

    useEffect(() => {
        if (listId) fetchDetail();
    }, [listId]);

    const handleRemoveMovie = async (movieId: number) => {

        if (!confirm('Bu filmi listeden çıkarmak istediğinize emin misiniz?')) return;

        try {
            await customListService.removeMovieFromList(listId, movieId);
            setList((prev) =>
                prev
                    ? {
                        ...prev,
                        movies: prev.movies.filter((m) => m.id !== movieId),
                        movieCount: prev.movieCount - 1,
                        }
                    : null
                );
        }
        catch (err) {
            alert('Film çıkarılamadı.');
        }
    };

    if (loading) {
        return <div className="min-h-screen bg-slate-950 text-white p-12">Yükleniyor...</div>;
    }

    if (!list) {
        return <div className="min-h-screen bg-slate-950 text-white p-12">Liste bulunamadı veya erişim izniniz yok.</div>;
    }

    const isOwner = user?.email === list.ownerEmail;

    return (
        <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
        <div className="max-w-6xl mx-auto space-y-8">
            <div>
            <button
                onClick={() => router.back()}
                className="text-xs text-slate-400 hover:text-white mb-4 block"
            >
                ← Geri Dön
            </button>
            <div className="flex justify-between items-start">
                <div>
                <h1 className="text-3xl font-black text-slate-100">{list.title}</h1>
                <p className="text-sm text-slate-400 mt-1">{list.description}</p>
                </div>
                <span className="text-xs px-3 py-1 rounded-full bg-slate-800 border border-slate-700 text-slate-300">
                {list.isPublic ? '🌐 Herkese Açık' : '🔒 Özel'}
                </span>
            </div>
            <div className="text-xs text-slate-500 mt-4 flex gap-4">
                <span>Oluşturan: <strong className="text-slate-300">{list.ownerEmail}</strong></span>
                <span>•</span>
                <span>{list.movieCount} Film</span>
            </div>
            </div>

            {list.movies.length === 0 ? (
            <div className="text-center py-20 bg-slate-900/40 rounded-2xl border border-slate-800">
                <p className="text-slate-400 text-sm">Bu listede henüz hiç film yok.</p>
            </div>
            ) : (
            <div className="grid grid-cols-2 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5 gap-6">
                {list.movies.map((movie) => (
                <div
                    key={movie.id}
                    className="group bg-slate-900 rounded-xl overflow-hidden border border-slate-800 flex flex-col justify-between relative"
                >
                    <div
                    onClick={() => router.push(`/movies/${movie.id}`)}
                    className="aspect-[2/3] w-full bg-slate-950 cursor-pointer overflow-hidden"
                    >
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

                    <div className="p-3 space-y-2">
                    <h3 className="font-bold text-xs text-slate-100 truncate">{movie.title}</h3>
                    {isOwner && (
                        <button
                        onClick={() => handleRemoveMovie(movie.id)}
                        className="w-full py-1.5 bg-rose-500/10 hover:bg-rose-500/20 text-rose-400 text-[11px] font-semibold rounded transition"
                        >
                        Listeden Çıkar
                        </button>
                    )}
                    </div>
                </div>
                ))}
            </div>
            )}
        </div>
        </div>
    );

}