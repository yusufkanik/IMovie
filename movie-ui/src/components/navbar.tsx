'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { useAuth } from '@/context/AuthContext';

export default function Navbar() {
  const pathname = usePathname();
  const { isAuthenticated, logout, user } = useAuth();

  const isActive = (path: string) =>
    pathname === path || pathname.startsWith(`${path}/`);

  return (
    <header className="bg-slate-900/80 backdrop-blur-md border-b border-slate-800 sticky top-0 z-50">
      <div className="max-w-7xl mx-auto px-6 h-16 flex items-center justify-between">

        {/* Logo */}
        <Link href="/" className="flex items-center gap-2">
          <span className="text-xl font-black bg-gradient-to-r from-indigo-400 to-rose-400 bg-clip-text text-transparent">
            IMovie
          </span>
        </Link>

        {/* Navigation */}
        <nav className="flex items-center gap-6 text-sm font-medium">

          
          <Link
            href="/movies"
            className={`transition-colors hover:text-white ${
              isActive('/movies')
                ? 'text-indigo-400 font-semibold'
                : 'text-slate-400'
            }`}
          >
            Katalog
          </Link>

          {isAuthenticated && (
            <>

            <Link
                href="/"
                className={`transition-colors hover:text-white ${
                  isActive('/recommendations')
                    ? 'text-indigo-400 font-semibold'
                    : 'text-slate-400'
                }`}
              >
                Ana Sayfa
              </Link>

              <Link
                href="/favorites"
                className={`transition-colors hover:text-white ${
                  isActive('/favorites')
                    ? 'text-indigo-400 font-semibold'
                    : 'text-slate-400'
                }`}
              >
                Favorilerim
              </Link>

              <Link
                href="/lists"
                className={`transition-colors hover:text-white ${
                  isActive('/lists')
                    ? 'text-indigo-400 font-semibold'
                    : 'text-slate-400'
                }`}
              >
                Listelerim
              </Link>

              <Link
                href="/profile"
                className={`transition-colors hover:text-white ${
                  isActive('/profile')
                    ? 'text-indigo-400 font-semibold'
                    : 'text-slate-400'
                }`}
              >
                Profilim
              </Link>
            </>
          )}
        </nav>

        {/* User / Auth */}
        <div className="flex items-center gap-4">
          {isAuthenticated ? (
            <div className="flex items-center gap-3">
              <span className="text-xs text-slate-400 hidden sm:inline">
                {user?.email || 'Kullanıcı'}
              </span>

              <button
                onClick={logout}
                className="px-3 py-1.5 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded-lg text-xs transition-colors cursor-pointer border border-slate-700"
              >
                Çıkış Yap
              </button>
            </div>
          ) : (
            <Link
              href="/login"
              className="px-3.5 py-1.5 bg-indigo-600 hover:bg-indigo-500 text-white rounded-lg text-xs font-semibold transition-colors"
            >
              Giriş Yap
            </Link>
          )}
        </div>

      </div>
    </header>
  );
}