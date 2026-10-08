import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Routes } from '@angular/router';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { QuotesComponent } from './quotes.component';

const routes: Routes = [{ path: '', component: QuotesComponent }];

@NgModule({
  declarations: [QuotesComponent],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    RouterModule.forChild(routes)
  ]
})
export class QuotesModule {}
