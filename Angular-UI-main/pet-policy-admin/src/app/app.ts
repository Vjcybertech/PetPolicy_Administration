import { Component } from '@angular/core';
import { ToastService, ToastMessage } from './services/toast.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
  standalone: false
})
export class App {
  title = 'PawShield Policy Administration';
  navItems = [
    { label: 'Dashboard', icon: 'dashboard', route: '/dashboard' },
    { label: 'Policies', icon: 'policy', route: '/policies' },
    { label: 'Quotes', icon: 'request_quote', route: '/quotes' },
    { label: 'Create Policy', icon: 'add_circle', route: '/create-policy' },
  ];
  sidebarOpen = true;

  constructor(public toastService: ToastService) {}

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  removeToast(id: number) {
    this.toastService.remove(id);
  }
}
