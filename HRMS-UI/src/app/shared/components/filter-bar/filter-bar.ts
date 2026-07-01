import { Component, Input, Output, EventEmitter, OnInit, OnDestroy, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FilterField, SortOption } from './filter-bar.model';
import { Subject, Subscription } from 'rxjs';
import { debounceTime } from 'rxjs/operators';
import {
  LucideSearch,
  LucideSlidersHorizontal,
  LucideChevronDown,
  LucideX,
  LucideCheck,
  LucideArrowUpDown
} from '@lucide/angular';

@Component({
  selector: 'app-filter-bar',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideSearch,
    LucideSlidersHorizontal,
    LucideChevronDown,
    LucideX,
    LucideCheck,
    LucideArrowUpDown
  ],
  templateUrl: './filter-bar.html',
  styleUrl: './filter-bar.css'
})
export class FilterBarComponent implements OnInit, OnDestroy {
  @Input() fields: FilterField[] = [];
  @Input() sortOptions: SortOption[] = [];
  
  // Current active selections (can be passed in from URL state query params)
  @Input() currentFilters: { [key: string]: any } = {};
  @Input() sortBy = '';
  @Input() descending = false;

  @Output() filterChange = new EventEmitter<{ [key: string]: any }>();
  @Output() sortChange = new EventEmitter<{ sortBy: string; descending: boolean }>();

  // Internal states
  activeDropdown: string | null = null;
  searchSubject = new Subject<string>();
  private searchSub?: Subscription;

  // Local values for form inputs
  localSearch = '';
  mobileDrawerOpen = false;

  ngOnInit() {
    this.localSearch = this.currentFilters['search'] || '';
    
    // Setup debounced keyword search
    this.searchSub = this.searchSubject.pipe(
      debounceTime(350)
    ).subscribe(val => {
      this.updateFilter('search', val);
    });
  }

  ngOnDestroy() {
    this.searchSub?.unsubscribe();
  }

  @HostListener('document:click')
  onDocumentClick() {
    this.closeDropdown();
  }

  // Toggle drop down popover
  toggleDropdown(key: string, event: Event) {
    event.stopPropagation();
    if (this.activeDropdown === key) {
      this.activeDropdown = null;
    } else {
      this.activeDropdown = key;
    }
  }

  closeDropdown() {
    this.activeDropdown = null;
  }

  // Check if a filter is active
  isFilterActive(key: string): boolean {
    const val = this.currentFilters[key];
    return val !== undefined && val !== null && val !== '';
  }

  // Get active filter count or display label
  getFilterValueLabel(field: FilterField): string {
    const val = this.currentFilters[field.key];
    if (val === undefined || val === null || val === '') return '';
    
    if (field.type === 'select' && field.options) {
      const opt = field.options.find(o => o.value === val);
      return opt ? opt.label : val.toString();
    }
    return val.toString();
  }

  // Active filter list for display badges
  getActiveFiltersList() {
    return this.fields.filter(f => this.isFilterActive(f.key)).map(f => ({
      key: f.key,
      label: f.label,
      displayVal: this.getFilterValueLabel(f)
    }));
  }

  // Update search term
  onSearchInput(value: string) {
    this.localSearch = value;
    this.searchSubject.next(value);
  }

  clearSearch() {
    this.localSearch = '';
    this.updateFilter('search', '');
  }

  // Update a specific filter and emit
  updateFilter(key: string, value: any) {
    const nextFilters = { ...this.currentFilters };
    if (value === null || value === undefined || value === '') {
      delete nextFilters[key];
    } else {
      nextFilters[key] = value;
    }
    this.currentFilters = nextFilters;
    this.filterChange.emit(this.currentFilters);
  }

  // Clear a specific filter
  clearFilter(key: string, event?: Event) {
    if (event) event.stopPropagation();
    this.updateFilter(key, null);
  }

  // Reset all filters
  resetAll() {
    this.localSearch = '';
    this.currentFilters = {};
    this.filterChange.emit(this.currentFilters);
  }

  // Apply sorting
  changeSort(fieldVal: string) {
    if (this.sortBy === fieldVal) {
      this.sortChange.emit({ sortBy: fieldVal, descending: !this.descending });
    } else {
      this.sortChange.emit({ sortBy: fieldVal, descending: false });
    }
    this.closeDropdown();
  }

  // Check if any filters are active
  hasAnyActiveFilters(): boolean {
    const filterKeys = this.fields.map(f => f.key);
    const activeData = Object.keys(this.currentFilters).some(k => filterKeys.includes(k) && this.isFilterActive(k));
    return activeData || (this.localSearch !== undefined && this.localSearch !== null && this.localSearch !== '');
  }

  openMobileDrawer() {
    this.mobileDrawerOpen = true;
  }

  closeMobileDrawer() {
    this.mobileDrawerOpen = false;
  }
}
